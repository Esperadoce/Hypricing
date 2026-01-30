cmake_minimum_required(VERSION 3.19)
project(hyprlangwrap LANGUAGES C CXX)

set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# Hide symbols by default (export only what you mark explicitly in your header)
set(CMAKE_C_VISIBILITY_PRESET hidden)
set(CMAKE_CXX_VISIBILITY_PRESET hidden)
set(CMAKE_VISIBILITY_INLINES_HIDDEN YES)

# Default to Release for single-config generators
if (NOT CMAKE_CONFIGURATION_TYPES AND NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release CACHE STRING "Build type" FORCE)
endif()

# ---- Dependencies: system-first, fallback to vendored submodules ----
set(HYPR_VENDOR_DIR "${CMAKE_CURRENT_SOURCE_DIR}/vendor")

find_package(PkgConfig REQUIRED)

# Try system packages first (Arch / distro packages)
pkg_check_modules(HYPRLANG QUIET IMPORTED_TARGET hyprlang)
pkg_check_modules(HYPRUTILS QUIET IMPORTED_TARGET hyprutils)

add_library(hyprlangwrap SHARED
  src/hyprlang_wrap.cpp
)

# C++23 (required for std::print / std::println used by hyprlang headers)
target_compile_features(hyprlangwrap PRIVATE cxx_std_23)
set_target_properties(hyprlangwrap PROPERTIES
  CXX_EXTENSIONS OFF
  OUTPUT_NAME "hyprlangwrap"
)

target_include_directories(hyprlangwrap
  PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/include
)

if (HYPRLANG_FOUND AND HYPRUTILS_FOUND)
  message(STATUS "Using system hyprlang/hyprutils via pkg-config")
  target_link_libraries(hyprlangwrap PRIVATE PkgConfig::HYPRLANG PkgConfig::HYPRUTILS)
else()
  message(STATUS "System hyprlang/hyprutils not found; falling back to vendored submodules")

  if (NOT EXISTS "${HYPR_VENDOR_DIR}/hyprlang/CMakeLists.txt")
    message(FATAL_ERROR "Missing vendor/hyprlang. Install system hyprlang or init submodules.")
  endif()
  if (NOT EXISTS "${HYPR_VENDOR_DIR}/hyprutils/CMakeLists.txt")
    message(FATAL_ERROR "Missing vendor/hyprutils. Install system hyprutils or init submodules.")
  endif()

  add_subdirectory("${HYPR_VENDOR_DIR}/hyprutils")
  add_subdirectory("${HYPR_VENDOR_DIR}/hyprlang")

  # Prefer namespaced targets if provided; fall back to plain names.
  if (TARGET hyprlang::hyprlang)
    set(_HYPRLANG_TGT hyprlang::hyprlang)
  elseif (TARGET hyprlang)
    set(_HYPRLANG_TGT hyprlang)
  else()
    message(FATAL_ERROR "Vendored hyprlang target not found (expected hyprlang::hyprlang or hyprlang).")
  endif()

  if (TARGET hyprutils::hyprutils)
    set(_HYPRUTILS_TGT hyprutils::hyprutils)
  elseif (TARGET hyprutils)
    set(_HYPRUTILS_TGT hyprutils)
  else()
    message(FATAL_ERROR "Vendored hyprutils target not found (expected hyprutils::hyprutils or hyprutils).")
  endif()

  target_link_libraries(hyprlangwrap PRIVATE ${_HYPRLANG_TGT} ${_HYPRUTILS_TGT})
endif()

# Size optimizations for Release
target_compile_options(hyprlangwrap PRIVATE
  $<$<CONFIG:Release>:-Os -ffunction-sections -fdata-sections>
)

target_link_options(hyprlangwrap PRIVATE
  $<$<CONFIG:Release>:-Wl,--gc-sections,--exclude-libs,ALL>
)

# Optional: if you want to ship deps next to the .so/.exe, enable $ORIGIN rpath.
# set_target_properties(hyprlangwrap PROPERTIES
#   BUILD_RPATH "\$ORIGIN"
#   INSTALL_RPATH "\$ORIGIN"
# )

install(TARGETS hyprlangwrap LIBRARY DESTINATION lib)
install(FILES include/hyprlang_wrap.h DESTINATION include)

# Tests
enable_testing()
add_executable(test_simple tests/test_simple.cpp)
target_link_libraries(test_simple PRIVATE hyprlangwrap)
add_test(NAME simple_test COMMAND test_simple)

# Debug: Check if files exist
message(STATUS "Checking config.conf: ${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/config.conf")
message(STATUS "Checking multiline-errors.conf: ${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/multiline-errors.conf")

set(_cfg1 "${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/config.conf")
set(_cfg2 "${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/multiline-errors.conf")

if (EXISTS "${_cfg1}" AND EXISTS "${_cfg2}")
  message(STATUS "Copying vendored hyprlang test configs into build/tests")
  file(COPY "${_cfg1}" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/tests")
  file(COPY "${_cfg2}" DESTINATION "${CMAKE_CURRENT_BINARY_DIR}/tests")
else()
  message(STATUS "Vendored hyprlang test configs not present; skipping copy")
endif()


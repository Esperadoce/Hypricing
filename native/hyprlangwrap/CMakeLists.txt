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

find_package(PkgConfig REQUIRED)
pkg_check_modules(HYPRLANG REQUIRED IMPORTED_TARGET hyprlang)

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

target_link_libraries(hyprlangwrap
  PRIVATE
    PkgConfig::HYPRLANG
)

# Size optimizations for Release
target_compile_options(hyprlangwrap PRIVATE
  $<$<CONFIG:Release>:-Os -ffunction-sections -fdata-sections>
)

target_link_options(hyprlangwrap PRIVATE
  $<$<CONFIG:Release>:-Wl,--gc-sections,--exclude-libs,ALL>
)

# Optional: if you want to ship deps next to the .so/.exe, enable $ORIGIN rpath.
# Uncomment if needed.
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


#copying test config files
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/config.conf DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/tests)
file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/vendor/hyprlang/tests/config/multiline-errors.conf DESTINATION ${CMAKE_CURRENT_BINARY_DIR}/tests)
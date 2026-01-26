cmake_minimum_required(VERSION 3.19)
project(hyprlangwrap LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

find_package(PkgConfig REQUIRED)

# hyprlang installs a hyprlang.pc (and depends on hyprutils)
pkg_check_modules(HYPRLANG REQUIRED IMPORTED_TARGET hyprlang)

add_library(hyprlangwrap SHARED
    src/hyprlang_wrap.cpp
)

target_include_directories(hyprlangwrap
    PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/include
)

target_link_libraries(hyprlangwrap
    PRIVATE
        PkgConfig::HYPRLANG
)

set_target_properties(hyprlangwrap PROPERTIES OUTPUT_NAME "hyprlangwrap")

install(TARGETS hyprlangwrap LIBRARY DESTINATION lib)
install(FILES include/hyprlang_wrap.h DESTINATION include)

cmake_minimum_required(VERSION 3.21)
set(CMAKE_CXX_STANDARD 17)

#libcurl http only
set(OPENSSL_ROOT_DIR /usr/local/opt/openssl)
set(HTTP_ONLY TRUE)

include(FetchContent)
FetchContent_Declare(googletest GIT_REPOSITORY git@github.com:google/googletest.git GIT_TAG release-1.11.0)
FetchContent_Declare(curl GIT_REPOSITORY git@github.com:curl/curl.git GIT_TAG curl-7_78_0)
FetchContent_MakeAvailable(googletest curl)
project(AsyncNetworkEngine)

set(LIB_DEPENDENCIES
    CURL::libcurl
)

include_directories(include)
set(SOURCES
    src/asyncnetworkengine/asyncnetworkengine.cpp
)
set(TESTS
    tests/asetests.cpp
)

add_library(asyncnetworkengine STATIC ${SOURCES})
target_link_libraries(asyncnetworkengine ${LIB_DEPENDENCIES})

# Test Environment
enable_testing()
add_executable(AsyncNetworkEngineTests ${TESTS})
target_link_libraries(AsyncNetworkEngineTests ${LIB_DEPENDENCIES} asyncnetworkengine gtest_main)
add_test(NAME AsyncNetworkEngineTests COMMAND AsyncNetworkEngineTests)
cmake_minimum_required(VERSION 3.21)
set(CMAKE_CXX_STANDARD 20)

#libcurl http only
if(WIN32)
    message("Setup for Windows32")
    set(CURL_USE_SCHANNEL ON)
    set(CURL_USE_OPENSSL OFF)
else()
    set(OPENSSL_ROOT_DIR /usr/local/opt/openssl)
endif()
set(HTTP_ONLY ON)
#googletest
# For Windows: Prevent overriding the parent project's compiler/linker settings
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)

include(FetchContent)
FetchContent_Declare(googletest GIT_REPOSITORY https://github.com/google/googletest.git GIT_TAG release-1.12.1)
FetchContent_Declare(curl GIT_REPOSITORY https://github.com/curl/curl.git GIT_TAG curl-7_85_0)
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

add_library(asyncnetworkengine ${SOURCES})
target_link_libraries(asyncnetworkengine ${LIB_DEPENDENCIES})

# Test Environment
enable_testing()
add_executable(AsyncNetworkEngineTests ${TESTS})
target_link_libraries(AsyncNetworkEngineTests PRIVATE ${LIB_DEPENDENCIES} asyncnetworkengine gtest_main)
add_test(NAME AsyncNetworkEngineTests COMMAND AsyncNetworkEngineTests)
cmake_minimum_required(VERSION 3.21)
# PROJECT config
project(asyncnetworkengine VERSION 0.3.2)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
option(asyncnetworkengine_BUILD_TESTS "Should build tests?" ON)
if(NOT DEFINED asyncnetworkengine_BUILD_TESTS)
    set(asyncnetworkengine_BUILD_TESTS ON)
endif()

# CURL dependency config
if(WIN32)
    message("Setup for Windows32")
    set(CURL_USE_SCHANNEL ON)
    set(CURL_USE_OPENSSL OFF)
else()
    set(OPENSSL_ROOT_DIR /usr/local/opt/openssl)
endif()
set(HTTP_ONLY ON)
set(BUILD_CURL_EXE OFF)
set(BUILD_SHARED_LIBS OFF)
# PROTOBUF dependency config
set(protobuf_INSTALL OFF)
set(protobuf_BUILD_TESTS OFF)
set(protobuf_BUILD_CONFORMANCE OFF)
set(protobuf_BUILD_EXAMPLES OFF)
set(protobuf_BUILD_PROTOC_BINARIES OFF)
set(protobuf_BUILD_LIBPROTOC OFF)
set(protobuf_MSVC_STATIC_RUNTIME OFF)
set(protobuf_BUILD_SHARED_LIBS OFF)
set(protobuf_USE_STATIC_LIBS ON)
# GOOGLETEST dependency config
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)

include(FetchContent)
FetchContent_Declare(curl GIT_REPOSITORY https://github.com/curl/curl.git GIT_TAG curl-7_85_0)
FetchContent_Declare(protobuf GIT_REPOSITORY https://github.com/protocolbuffers/protobuf.git GIT_TAG v23.4)
FetchContent_Declare(googletest GIT_REPOSITORY https://github.com/google/googletest.git GIT_TAG release-1.12.1)
FetchContent_MakeAvailable(googletest curl protobuf)

set(LIB_DEPENDENCIES
    CURL::libcurl
    protobuf::libprotobuf-lite
)

set(SOURCES
    src/ase/asyncnetworkengine.cpp
    src/ase/asyncrpc.pb.cc
    src/ase/base64.cpp
)
set(TESTS
    tests/asetests.cpp
)

add_library(asyncnetworkengine STATIC ${SOURCES})
target_include_directories(asyncnetworkengine PUBLIC ${CMAKE_CURRENT_LIST_DIR}/src)
target_link_libraries(asyncnetworkengine ${LIB_DEPENDENCIES})

# Test Environment
if(asyncnetworkengine_BUILD_TESTS)
    enable_testing()
    add_executable(AsyncNetworkEngineTests ${TESTS})
    target_link_libraries(AsyncNetworkEngineTests PRIVATE ${LIB_DEPENDENCIES} asyncnetworkengine gtest_main)
    add_test(NAME AsyncNetworkEngineTests COMMAND AsyncNetworkEngineTests)
endif()
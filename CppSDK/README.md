# Async Network Engine

Application Http requests made easy to interoperability with Go Lang Backend + Google Protobuf functions!

Supported Cloud Functions:
- Google Cloud Functions
- Amazon Web Services Lambdas

Use at your own risk!

## Supported Platforms

* MacOS
* Windows (pending testing)
* Linux (pending testing)
* iOS (pending testing)
* Android (pending testing)

## How to install

Async Network Engine is prepared to be used with CMake.
At your CMakeLists.txt, fetch the repository using:
```cmake
include(FetchContent)
FetchContent_Declare(
    asyncnetworkengine
    GIT_REPOSITORY git@github.com:GameWorkstore/async-network-engine-cpp.git
    GIT_TAG 0.0.1)
FetchContent_MakeAvailable(asyncnetworkengine)
```

At your target, add asyncnetworkengine library:
```cmake
add_executable(MyExecutable src/main.cpp)
target_include_directories(MyExecutable PRIVATE ${AsyncNetworkEngine_SOURCE_DIR}/include)
target_link_libraries(MyExecutable asyncnetworkengine)
```

Save and generate the CMake project.

To update package for a newer version, update GIT_TAG from 0.0.1 to any released version on Releases.

## Usage examples

You can find usage examples on tests/ folder, for each cloud.

# Https Server

Async Network Engine Cpp is the client http. For communication, is required to implement GoLang http server. Head to origin repo for info: https://github.com/GameWorkstore/async-network-engine

# Contributions

If you are using this library and want to submit a change, go ahead! Overall, this project accepts contributions if:
- Is a bug fix;
- Or, is a generalization of a well-known issue;
- Or is performance improvement;
- Or is an improvement to already supported feature.

Also, you can donate to allow us to drink coffee while we improve it for you!

OBS: This repository don't accept contributions directly. you must submit contributions to the origin (https://github.com/GameWorkstore/async-network-engine), checkout main branch and PR to there.

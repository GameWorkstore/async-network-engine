# Async Network Engine Go

Unity http requests made easy to interoperability with Go Lang + Google Protobuf functions!

Supported Cloud Functions:
- Google Cloud Functions
- Amazon Web Services Lambdas

Use at your own risk!

# Repository

[Async Network Engine Go](https://github.com/GameWorkstore/async-network-engine-go) is a go module developed side-by-side with UnityEngine open-source module [Async Network Engine](https://github.com/GameWorkstore/async-network-engine).
Is intended to be used together, but it suitable be used with any custom GoogleProtobuf client.

# How to install

At go.mod, add async network engine:
```json
module example.com/module

go 1.13

require github.com/GameWorkstore/async-network-engine-go v0.1.3
```
and execute
> go mod tidy

to download it into your machine.
or use
> go get github.com/GameWorkstore/async-network-engine-go

on the case want to use it in many projects.

# Future Work

Integrate with gRPC may be the natural evolution of this package, but it's production ready at current state.

# Contributions

If you are using this library and want to submit a change, go ahead! Overall, this project accepts contributions if:
- Is a bug fix;
- Or, is a generalization of a well-known issue;
- Or is performance improvement;
- Or is an improvement to already supported feature.

Also, you can donate to allow us to drink coffee while we improve it for you!

# Async Network Engine

Unity http requests made easy to interoperability with Go Lang + Google Protobuf functions!

Supported Cloud Functions:
- Google Cloud Functions
- Amazon Web Services Lambdas

Use at your own risk!

# How to install

At package.json, add these 3 lines of code:
```json
"com.gameworkstore.asyncnetworkengine": "git://github.com/GameWorkstore/async-network-engine.git",
"com.gameworkstore.googleprotobufunity": "git://github.com/GameWorkstore/google-protobuf-unity.git",
"com.gameworkstore.patterns": "git://github.com/GameWorkstore/patterns.git"
```

And wait for unity to download and compile the package.

for update package for a newer version, install UpmGitExtension and update on [ Window > Package Manager ]!
> https://github.com/mob-sakai/UpmGitExtension

# Usage Examples

You can find usage examples on Assets/Tests/ folder, for each cloud.

## Uploading to Google Cloud Functions

on upmsync, the job 'upload_gcp' illustrates a possible way to upload your functions into GCP.
Requires a service account to enable it to upload.

## Uploading to Amazon Web Services

on upmsync.yaml, the job 'upload_aws' illustrates a possible way to upload your functions into AWS.
Requires a service account to enable it to upload.
> You need to set the template 'cloudformation_function.yaml' public in your bucket repository to enable AWS::Stack to read from it.

# Implementation

## GCP Troubleshoot

> My function is returning ErrorProtocol for any input.
if you don't give access public for your function it might fail

## AWS Troubleshoot
> CloudFormation is returning errors

Verify all variables, !Ref and links, you might be forgetting something. CloudFormations is very sensitive to linkage errors.

> My lambda is returning error 500 Internal Server error.
If you are receiving this and error object is returning null, it might be a bad configuration causing your lambda to not run.
Verify if you function is running by adding a fmt.Println("test") at main() function to ensure the function is starting.
Some issues that may prevent the start of function:
- The functions ins't at correct path when extracted from the zip.
- The function package name isn't main where main() function is declared.
- lambda.Start() is never begin called to initialize the function.
- The function is crashing upon initialization

If the function is working normally, them you might receive error 500 - ErrorInternalServer with AWSError, when specified by the programmer.

# Contributions

If you are using this library and want to submit a change, go ahead! Overall, this project accepts contributions if:
- Is a bug fix;
- Or, is a generalization of a well-known issue;
- Or is performance improvement;
- Or is an improvement to already supported feature.

Also, you can donate to allow us to drink coffee while we improve it for you!

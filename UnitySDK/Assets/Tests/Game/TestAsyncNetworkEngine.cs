
using GameWorkstore.AsyncNetworkEngine;
using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;
using System;
using Google.Protobuf;
using System.IO;
using System.Linq;

public class TestAsyncNetworkEngine : MonoBehaviour
{
    private const string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";
    private const string gcp = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";
    private const string aws = "https://c9dil5kv2d.execute-api.us-east-1.amazonaws.com/default/aseawstest";
    private const string aws_binary = "https://c9dil5kv2d.execute-api.us-east-1.amazonaws.com/default/asebinaryconversions";
    private const string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/cloudformation_function.yaml";
    private static string aws_local_file { get { return Path.Combine(Application.streamingAssetsPath,"cloudformation_function.yaml"); } }
    private static string aws_cache_file { get { return Path.Combine(Application.persistentDataPath,"../Cache/"); } }

    private void Awake()
    {
        AsyncNetworkEngineMap.SetupCloudMap(new Dictionary<string, CloudProvider>()
        {
            { "https://us-central1-game-workstore", CloudProvider.Gcp },
            { "https://c9dil5kv2d", CloudProvider.Aws }
        });

        AWS_Download();
        AWS_DownloadAll();
        //AWS_DownloadFileToCache();
        AWS_Success();
        AWS_Errors();
        AWS_Binary();
        AWS_BinaryLarge();
        GCP_NotAuthorized();
        GCP_Success();
        GCP_Errors();
    }

    private void AWS_Download()
    {
        AsyncNetworkEngine.Download(aws_remote_file, (file) =>
        {
            Assert.AreEqual(Transmission.Success, file.Result);
            Assert.IsNotNull(file.Data);

            var comparing = File.ReadAllBytes(aws_local_file);
            Assert.IsTrue(comparing.SequenceEqual(file.Data));
            Debug.Log(nameof(AWS_Download));
        });
    }

    private void AWS_DownloadAll()
    {
        var files = new[]{
            aws_remote_file,
            aws_remote_file,
            aws_remote_file
        };

        AsyncNetworkEngine.Download(files, (progress) =>
        {
            Assert.IsNotNull(progress.Files);
            Assert.AreNotEqual(0, progress.Files.Length);
            foreach (FileData file in progress.Files)
            {
                Assert.AreEqual(Transmission.Success, file.Result);
                Assert.IsNotNull(file.Data);

                var comparing = File.ReadAllBytes(aws_local_file);
                Assert.IsTrue(comparing.SequenceEqual(file.Data));
                Debug.Log(nameof(AWS_Download));
            }
        });
    }

    /* Future Work:
    private void AWS_DownloadFileToCache()
    {
        AsyncNetworkEngine.DownloadToCache(aws_remote_file,aws_cache_file,(result) => {

        });
    }*/

    private void AWS_Binary()
    {
        const string msg = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean fermentum arcu in sem lacinia, nec fermentum ligula porttitor. Morbi egestas sem non odio convallis volutpat. In arcu diam, finibus elementum eleifend semper, porta vitae ex. Pellentesque accumsan lobortis interdum. Vivamus mi eros, ultricies vitae varius id, maximus vel velit. Nam condimentum nibh eu tortor ultricies suscipit. Fusce quis nulla ante. Aenean sodales neque ac nulla vehicula tempus. Sed non elit nec nisi scelerisque mollis. Pellentesque tristique tellus eros, sed tincidunt nibh cursus at.";
        var rqt = new GenericRequest()
        {
            Messege = msg
        };

        AsyncNetworkEngine<GenericRequest,GenericRequest>.Send(aws_binary,rqt,(result,response,error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual(msg, response.Messege);
            Debug.Log(nameof(AWS_Binary));
        });
    }

    private void AWS_BinaryLarge()
    {
        const string msg = 
            "Lorem ipsum dolor sit amet, consectetur adipiscing eli()t. Aenean fermentum arcu in sem lacinia, nec fermentum ligula porttitor. Morbi egestas sem non odio convallis volutpat. In arcu diam, finibus elementum eleifend semper, porta vitae ex. Pellentesque accumsan lobortis interdum. Vivamus mi eros, ultricies vitae varius id, maximus vel velit. Nam condimentum nibh eu tortor ultricies suscipit. Fusce quis nulla ante. Aenean sodales neque ac nulla vehicula tempus. Sed non elit nec nisi scelerisque mollis. Pellentesque tristique tellus eros, sed tincidunt nibh cursus at.\n"+
            "Duis at aliquet lectus, non s**tempus erat. Ut sagittis diam- porta pellentesque luctus. Quisque sit amet erat vitae est feugiat posuere. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum at sodales metus, nec fringilla ipsum. Sed magna risus, facilisis sit amet viverra sed, mattis laoreet risus. Morbi malesuada ligula et rhoncus volutpat.\n"+
            "Proin ullamcorper lectus23// nec venenatis dapibus. Etiam porttitor m-agna ut mi pellentesque efficitur. Suspendisse non neque eu elit malesuada ultricies. Sed vel tellus purus. Fusce eu ante felis. Quisque eu imperdiet risus, ut accumsan augue. In hac habitasse platea dictumst. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Vestibulum at libero congue, convallis sapien id, hendrerit augue. Donec dignissim dignissim mauris at efficitur. Phasellus sit amet ipsum id elit mollis consequat.\n"+
            "Donec dui augue, ultrices eget commodo sed, iaculis id erat.\n"+
            "Donec aliquet ipsum ut diam accumsan mollis.\n"+
            "Praesent eget% massa sed velit condimentum molestie. Nullam lectu+=s t))orto&&r, pretium sit amet congue vitae, mollis ut tortor. Nullam ultricies risus urna. In varius congue nulla a dignissim. Pellentesque eu dolor vel dui congue tincidunt. In malesuada tincidunt nunc id fermentum. Etiam rutrum, mi lacinia finibus tincidunt, erat erat ultrices felis, non tempor eros ligula vitae ligula. Proin aliquet, neque a placerat dignissim, nisi nulla pharetra ipsum, id interdum urna urna eu elit. Mauris velit neque, dictum ac vulputate id, mattis eget velit.\n"+
            "Nam ut feugiat diam. Nunc non !sapien @#auctor, #elementum magna non, plac6$$erat lorem. In at est ac lorem consequat dignissim ac nec tellus. Duis vitae semper metus, quis ornare orci. Praesent nec leo sollicitudin, porttitor risus vitae, vestibulum ex. Fusce a lobortis arcu, eu imperdiet metus. Aliquam ornare orci at accumsan aliquam. Aenean feugiat bibendum tortor, ut interdum augue ultrices in. Proin quis placerat eros. Integer tempus iaculis condimentum. Vivamus euismod arcu ac lacus commodo, a scelerisque lectus venenatis. Mauris non massa tortor. Cras id nulla ac orci lobortis ullamcorper eu at quam.\n"+
            "Donec nibh elit, placerat a consequat vitae, porta accumsan metus.";
        var rqt = new GenericRequest()
        {
            Messege = msg
        };

        AsyncNetworkEngine<GenericRequest,GenericRequest>.Send(aws_binary,rqt,(result,response,error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual(msg, response.Messege);
            Debug.Log(nameof(AWS_BinaryLarge));
        });
    }

    public void GCP_NotAuthorized()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp_notauthorized, rqt, (result, response, error) =>
        {
            DebugResult(nameof(GCP_NotAuthorized), result, response, error);
        });
    }

    public void GCP_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp, rqt, (result, response, error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual("received-success", response.Messege);
            Debug.Log(nameof(GCP_Success));
        });
    }

    private void GCP_Errors()
    {
        var tuples = new[]
        {
            new Tuple<Transmission,string,string>(Transmission.ErrorDecode,"decode-error","decode error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorEncode,"encode-error","encode error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorInternalServer,"internal-error","internal error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorMethodNotAllowed,"not-allowed-error","not allowed error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorNotImplemented,"not-implemented","not implemented"),
        };

        foreach (var t in tuples)
        {
            var tuple = t;
            var rqt = new GenericRequest()
            {
                Messege = tuple.Item2
            };
            AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp, rqt, (result, response, error) =>
            {
                //DebugResult(nameof(GCP_Decode_Error),result,response,error);
                Assert.AreEqual(tuple.Item1, result);
                Assert.IsNotNull(error);
                Assert.AreEqual(tuple.Item3, error.Error);
                Debug.Log(nameof(GCP_Errors) + ":" + tuple.Item1);
            });
        }
    }

    public void AWS_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(aws, rqt, (result, response, error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual("received-success", response.Messege);
            Debug.Log(nameof(AWS_Success));
        });
    }

    public void AWS_Errors()
    {
        var tuples = new[]
        {
            new Tuple<Transmission,string,string>(Transmission.ErrorDecode,"decode-error","decode error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorEncode,"encode-error","encode error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorInternalServer,"internal-error","internal error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorMethodNotAllowed,"not-allowed-error","not allowed error"),
            new Tuple<Transmission,string,string>(Transmission.ErrorNotImplemented,"not-implemented","not implemented"),
        };

        foreach (var t in tuples)
        {
            var tuple = t;
            var rqt = new GenericRequest()
            {
                Messege = tuple.Item2
            };
            AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(aws, rqt, (result, response, error) =>
            {
                //DebugResult(nameof(GCP_Decode_Error),result,response,error);
                Assert.AreEqual(tuple.Item1, result);
                Assert.IsNotNull(error);
                Assert.AreEqual(tuple.Item3, error.Error);
                Debug.Log(nameof(AWS_Errors) + ":" + tuple.Item1);
            });
        }
    }

    private static void DebugResult(string function, Transmission result, IMessage response, IMessage error)
    {
        var debugged =
            "Function:" + function + ":\n" +
            "Result:" + result + "\n" +
            "Response:" + (response != null ? response.ToString() : "null") + "\n" +
            "Error:" + (error != null ? error.ToString() : "null");
        Debug.Log(debugged);
    }
}

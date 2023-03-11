using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameWorkstore.AsyncNetworkEngine;
using GameWorkstore.Patterns;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.Networking.UnityWebRequest;

public class EditorModeTest
{
    private const string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";
    private const string gcp = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    private const string aws = "https://c9dil5kv2d.execute-api.us-east-1.amazonaws.com/default/aseawstest";
    private const string aws_binary = "https://c9dil5kv2d.execute-api.us-east-1.amazonaws.com/default/asebinaryconversions";
    private const string aws_remote_file = "https://ase-test-bucket.s3.amazonaws.com/cloudformation_function.yaml";
    private static string aws_local_file { get { return Path.Combine(Application.streamingAssetsPath, "cloudformation_function.yaml"); } }
    private static string aws_cache_file { get { return Path.Combine(Application.persistentDataPath, "../Cache/"); } }

    [OneTimeSetUp]
    public void Setup()
    {
        AsyncNetworkEngineMap.SetupCloudMap(new Dictionary<string, CloudProvider>()
        {
            { "https://us-central1-game-workstore", CloudProvider.Gcp },
            { "https://c9dil5kv2d", CloudProvider.Aws }
        });
    }

    [Test]
    public void ConvertBase64OnlyIfValid()
    {
        const string dt = "packed-string-data";
        var bytes = Encoding.UTF8.GetBytes(dt);
        var base64 = Base64StdEncoding.Encode(bytes);

        var result = Base64StdEncoding.Decode(base64, out var data);
        Assert.True(result);
        Assert.IsTrue(data.SequenceEqual(bytes));

        base64 += "-#42$$";

        result = Base64StdEncoding.Decode(base64, out data);
        Assert.False(result);
    }

    [UnityTest]
    public IEnumerator DownloadManyFiles()
    {
        var files = new string[]
        {
            "https://phyengine.com/content/base/logomark.png",
            "https://phyengine.com/content/favicon/android-chrome-192x192.png",
            "https://phyengine.com/content/favicon/android-chrome-512x512.png",
            "https://phyengine.com/content/favicon/apple-touch-icon.png",
            "https://phyengine.com/content/favicon/favicon-16x16.png",
            "https://phyengine.com/content/favicon/favicon-32x32.png",
            "https://phyengine.com/content/favicon/favicon.ico",
            "https://phyengine.com/content/footer/icon_sm_phyengine.png",
            "https://phyengine.com/content/footer/icon_sm_phyrexi.png",
            "https://phyengine.com/content/header/header_gradient.png",
            "https://phyengine.com/content/header/header_icon.png",
            "https://phyengine.com/content/home/video-loop.png",
            "https://phyengine.com/content/home/video-loop.png"
        };

        var result = false;
        var gate = new Gate();
        AsyncNetworkEngine.Send(files,
        (progression) =>
        {
            Debug.Log($"completed: {progression}");
            result = progression.Progress >= 1;
            gate.Release();
        },
        (progression) =>
        {
            Debug.Log($"inprogress: {progression}");
        });
        yield return gate;
        Assert.AreEqual(true, result);
    }

    //[UnityTest]
    public IEnumerator AWS_DownloadOneFile()
    {
        var gate = new Gate();
        AsyncNetworkEngine.Download(aws_remote_file, (file) =>
        {
            Assert.AreEqual(Transmission.Success, file.Result);
            Assert.IsNotNull(file.Data);

            var comparing = File.ReadAllBytes(aws_local_file);
            Assert.IsTrue(comparing.SequenceEqual(file.Data));
            gate.Release();
        });
        yield return gate;
    }

    //[UnityTest]
    public IEnumerator AWS_DownloadAllFiles()
    {
        var files = new[]{
            aws_remote_file,
            aws_remote_file,
            aws_remote_file
        };

        var gate = new Gate();
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
            }
            gate.Release();
        });
        yield return gate;
    }

    /* Future Work:
    private void AWS_DownloadFileToCache()
    {
        AsyncNetworkEngine.DownloadToCache(aws_remote_file,aws_cache_file,(result) => {

        });
    }*/

    [UnityTest]
    public IEnumerator AWS_Binary()
    {
        const string msg = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean fermentum arcu in sem lacinia, nec fermentum ligula porttitor. Morbi egestas sem non odio convallis volutpat. In arcu diam, finibus elementum eleifend semper, porta vitae ex. Pellentesque accumsan lobortis interdum. Vivamus mi eros, ultricies vitae varius id, maximus vel velit. Nam condimentum nibh eu tortor ultricies suscipit. Fusce quis nulla ante. Aenean sodales neque ac nulla vehicula tempus. Sed non elit nec nisi scelerisque mollis. Pellentesque tristique tellus eros, sed tincidunt nibh cursus at.";
        var rqt = new GenericRequest()
        {
            Messege = msg
        };
        
        var gate = new Gate();
        AsyncNetworkEngine<GenericRequest, GenericRequest>.Send(aws_binary, rqt, (result, response, error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual(msg, response.Messege);
            Debug.Log(nameof(AWS_Binary));
            gate.Release();
        });
        yield return gate;
    }

    [UnityTest]
    public IEnumerator AWS_BinaryLarge()
    {
        const string msg =
            "Lorem ipsum dolor sit amet, consectetur adipiscing eli()t. Aenean fermentum arcu in sem lacinia, nec fermentum ligula porttitor. Morbi egestas sem non odio convallis volutpat. In arcu diam, finibus elementum eleifend semper, porta vitae ex. Pellentesque accumsan lobortis interdum. Vivamus mi eros, ultricies vitae varius id, maximus vel velit. Nam condimentum nibh eu tortor ultricies suscipit. Fusce quis nulla ante. Aenean sodales neque ac nulla vehicula tempus. Sed non elit nec nisi scelerisque mollis. Pellentesque tristique tellus eros, sed tincidunt nibh cursus at.\n" +
            "Duis at aliquet lectus, non s**tempus erat. Ut sagittis diam- porta pellentesque luctus. Quisque sit amet erat vitae est feugiat posuere. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum at sodales metus, nec fringilla ipsum. Sed magna risus, facilisis sit amet viverra sed, mattis laoreet risus. Morbi malesuada ligula et rhoncus volutpat.\n" +
            "Proin ullamcorper lectus23// nec venenatis dapibus. Etiam porttitor m-agna ut mi pellentesque efficitur. Suspendisse non neque eu elit malesuada ultricies. Sed vel tellus purus. Fusce eu ante felis. Quisque eu imperdiet risus, ut accumsan augue. In hac habitasse platea dictumst. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Vestibulum at libero congue, convallis sapien id, hendrerit augue. Donec dignissim dignissim mauris at efficitur. Phasellus sit amet ipsum id elit mollis consequat.\n" +
            "Donec dui augue, ultrices eget commodo sed, iaculis id erat.\n" +
            "Donec aliquet ipsum ut diam accumsan mollis.\n" +
            "Praesent eget% massa sed velit condimentum molestie. Nullam lectu+=s t))orto&&r, pretium sit amet congue vitae, mollis ut tortor. Nullam ultricies risus urna. In varius congue nulla a dignissim. Pellentesque eu dolor vel dui congue tincidunt. In malesuada tincidunt nunc id fermentum. Etiam rutrum, mi lacinia finibus tincidunt, erat erat ultrices felis, non tempor eros ligula vitae ligula. Proin aliquet, neque a placerat dignissim, nisi nulla pharetra ipsum, id interdum urna urna eu elit. Mauris velit neque, dictum ac vulputate id, mattis eget velit.\n" +
            "Nam ut feugiat diam. Nunc non !sapien @#auctor, #elementum magna non, plac6$$erat lorem. In at est ac lorem consequat dignissim ac nec tellus. Duis vitae semper metus, quis ornare orci. Praesent nec leo sollicitudin, porttitor risus vitae, vestibulum ex. Fusce a lobortis arcu, eu imperdiet metus. Aliquam ornare orci at accumsan aliquam. Aenean feugiat bibendum tortor, ut interdum augue ultrices in. Proin quis placerat eros. Integer tempus iaculis condimentum. Vivamus euismod arcu ac lacus commodo, a scelerisque lectus venenatis. Mauris non massa tortor. Cras id nulla ac orci lobortis ullamcorper eu at quam.\n" +
            "Donec nibh elit, placerat a consequat vitae, porta accumsan metus.";
        var rqt = new GenericRequest()
        {
            Messege = msg
        };
        
        var gate = new Gate();
        AsyncNetworkEngine<GenericRequest, GenericRequest>.Send(aws_binary, rqt, (result, response, error) =>
        {
            Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual(msg, response.Messege);
            Debug.Log(nameof(AWS_BinaryLarge));
            gate.Release();
        });
        yield return gate;
    }

    [UnityTest]
    public IEnumerator GCP_Unauthorized()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        var result = false;
        var gate = new Gate();
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp_notauthorized, rqt, (transmission, response, error) =>
        {
            result = transmission == Transmission.Success;
            gate.Release();
        });
        yield return gate;
        Assert.AreEqual(false, result);
    }

    [UnityTest]
    public IEnumerator GCP_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        Transmission result = Transmission.NotSpecified;
        GenericResponse response = null;
        var gate = new Gate();
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp, rqt, (transmission, resp, error) =>
        {
            result = transmission;
            response = resp;
            Debug.Log(nameof(GCP_Success));
            gate.Release();
        });
        yield return gate;
        Assert.AreEqual(Transmission.Success, result);
        Assert.IsNotNull(response);
        Assert.AreEqual("received-success", response.Messege);
    }

    public void GCP_Errors()
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

    [UnityTest]
    public IEnumerator AWS_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        Transmission result = Transmission.NotSpecified;
        GenericResponse response = null;
        var gate = new Gate();
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(aws, rqt, (transmission, resp, error) =>
        {
            result = transmission;
            response = resp;
            Debug.Log(nameof(AWS_Success));
            gate.Release();
        });
        yield return gate;
        Assert.AreEqual(Transmission.Success, result);
        Assert.IsNotNull(response);
        Assert.AreEqual("received-success", response.Messege);
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
}

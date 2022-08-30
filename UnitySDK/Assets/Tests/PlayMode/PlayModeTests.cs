using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameWorkstore.AsyncNetworkEngine;
using GameWorkstore.Patterns;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTests
{

    private const string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";

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
    public IEnumerator SendRequest_CallSendRequestExecutesUntilTheEnd_IsCalled()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        var gate = new Gate();
        yield return AsyncNetworkEngine<GenericRequest, GenericResponse>.SendRequest(gcp_notauthorized, rqt, (result, response, error) =>
        {
            gate.Release();
        });
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
        yield return AsyncNetworkEngine.SendRequest(files,
        (progression) =>
        {
            result = progression.Progress >= 1;
        },
        (progression) =>
        {
            Debug.Log(progression.ToString());
        });
        Assert.AreEqual(true, result);
    }

}

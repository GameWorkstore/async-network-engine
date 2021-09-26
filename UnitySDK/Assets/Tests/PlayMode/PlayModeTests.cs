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
}

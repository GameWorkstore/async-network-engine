using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameWorkstore.AsyncNetworkEngine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTests
{
    // A Test behaves as an ordinary method
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
}

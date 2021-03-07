
using GameWorkstore.AsyncNetworkEngine;
using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;
using System;
using Google.Protobuf;

public class TestAsyncNetworkEngine : MonoBehaviour
{
    private const string url = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    private void Awake()
    {
        AsyncNetworkEngineMap.SetupCloudMap(new Dictionary<string, CloudProvider>()
        {
            {"https://us-central1-game-workstore", CloudProvider.GCP }
        });
        GCP_Success();
        GCP_Decode_Error();
        AWS_Success();
    }

    public void GCP_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(url, rqt, (result, response, error) =>
        {
            Assert.AreEqual(AsyncNetworkResult.SUCCESS, result);
            Assert.IsNotNull(response);
            Assert.AreEqual("received-success", response.Messege);
            Debug.Log("GCP_Success:True");
        });
    }

    private void GCP_Decode_Error()
    {
        var rqt = new GenericRequest()
        {
            Messege = "decode-error"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(url, rqt, (result, response, error) =>
        {
            DebugResult(result,response,error);
            //Assert.AreEqual(AsyncNetworkResult.SUCCESS, result);
            //Assert.IsNotNull(response);
            //Assert.AreEqual("received-success", response.Messege);
            //Debug.Log("GCP_Decode_Error:True");
        });
    }

    public void AWS_Success()
    {

    }

    private void DebugResult(AsyncNetworkResult result, IMessage response, IMessage error)
    {
        Debug.Log(result);
        Debug.Log(response != null ? response.ToString() : "null");
        Debug.Log(error != null ? error.ToString() : "null");
    }
}

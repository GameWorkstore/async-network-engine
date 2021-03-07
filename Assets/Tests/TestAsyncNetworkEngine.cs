
using GameWorkstore.AsyncNetworkEngine;
using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;
using System;
using Google.Protobuf;

public class TestAsyncNetworkEngine : MonoBehaviour
{
    private const string gcp_notauthorized = "https://us-central1-game-workstore.cloudfunctions.net/gcptest-notauthorized";
    private const string gcp = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    private void Awake()
    {
        AsyncNetworkEngineMap.SetupCloudMap(new Dictionary<string, CloudProvider>()
        {
            {"https://us-central1-game-workstore", CloudProvider.GCP }
        });
        GCP_NonExisting();
        GCP_Success();
        GCP_Decode_Error();
        AWS_Success();
    }

    public void GCP_NonExisting()
    {
        return;
        /*var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp_notauthorized, rqt, (result, response, error) =>
        {
            DebugResult(nameof(GCP_NonExisting),result,response,error);
            /*Assert.AreEqual(Transmission.Success, result);
            Assert.IsNotNull(response);
            Assert.AreEqual("received-success", response.Messege);
            Debug.Log("GCP_Success:True");
        });*/
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
            Debug.Log("GCP_Success:True");
        });
    }

    private void GCP_Decode_Error()
    {
        var rqt = new GenericRequest()
        {
            Messege = "decode-error"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp, rqt, (result, response, error) =>
        {
            DebugResult(nameof(GCP_Decode_Error),result,response,error);
            //Assert.AreEqual(AsyncNetworkResult.SUCCESS, result);
            //Assert.IsNotNull(response);
            //Assert.AreEqual("received-success", response.Messege);
            //Debug.Log("GCP_Decode_Error:True");
        });
    }

    public void AWS_Success()
    {

    }

    private void DebugResult(string function,Transmission result, IMessage response, IMessage error)
    {
        string debugged = 
            "Function:" + function + ":\n" +
            "Result:" + result + "\n" +
            "Response:" + (response != null ? response.ToString() : "null") + "\n" +
            "Error:" + (error != null ? error.ToString() : "null");
        Debug.Log(debugged);
    }
}

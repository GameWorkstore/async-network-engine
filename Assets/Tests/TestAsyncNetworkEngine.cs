
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
        GCP_NotAuthorized();
        GCP_Success();
        GCP_Decode_Error();
        AWS_Success();
    }

    public void GCP_NotAuthorized()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcp_notauthorized, rqt, (result, response, error) =>
        {
            DebugResult(nameof(GCP_NotAuthorized),result,response,error);
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

    private void GCP_Decode_Error()
    {
        var tuples = new Tuple<Transmission, string, string>[]
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
                Debug.Log(nameof(GCP_Decode_Error)+":"+tuple.Item1);
            });
        }
    }

    public void AWS_Success()
    {
        
    }

    private void DebugResult(string function, Transmission result, IMessage response, IMessage error)
    {
        string debugged =
            "Function:" + function + ":\n" +
            "Result:" + result + "\n" +
            "Response:" + (response != null ? response.ToString() : "null") + "\n" +
            "Error:" + (error != null ? error.ToString() : "null");
        Debug.Log(debugged);
    }
}

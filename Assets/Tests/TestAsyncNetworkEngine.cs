
using GameWorkstore.AsyncNetworkEngine;
using UnityEngine.Assertions;
using UnityEngine;

public class TestAsyncNetworkEngine : MonoBehaviour
{
    private const string url = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    private void Awake()
    {
        GCP_Success();
        AWS_Success();
    }

    public void GCP_Success()
    {
        var rqt = new GenericRequest()
        {
            Messege = "success"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Cloud = AsyncNetworkEngineCloud.GCP;
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(url, rqt, OnGCP_Success);
    }

    private void OnGCP_Success(AsyncNetworkResult result, GenericResponse response, GenericErrorResponse error)
    {
        Assert.AreEqual(AsyncNetworkResult.SUCCESS, result);
    }

    public void AWS_Success()
    {

    }
}

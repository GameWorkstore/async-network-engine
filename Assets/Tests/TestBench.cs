using UnityEngine;
using GameWorkstore.AsyncNetworkEngine;

public class TestBench : MonoBehaviour
{
    private const string gcptestendpoint = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    private void Awake()
    {
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Cloud = AsyncNetworkEngineCloud.GCP;
        var rqt = new GenericRequest()
        {
            Messege = "Content12345678"
        };
        AsyncNetworkEngine<GenericRequest, GenericResponse>.Send(gcptestendpoint, rqt,
            (asyncResult, resp, error) =>
            {
                Debug.Log("AsyncResult:" + asyncResult);
                Debug.Log("Error:" + error != null ? error.ToString() : "null");
                Debug.Log("Received:" + resp != null ? resp.ToString() : "null");
            });
    }
}

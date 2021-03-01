using System.Collections;
using GameWorkstore.AsyncNetworkEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

public class TestScript
{
    private const string gcptestendpoint = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    [UnityTest]
    public IEnumerator TestGCP()
    {

        AsyncNetworkEngine<GenericRequest, GenericResponse>.Cloud = AsyncNetworkEngineCloud.GCP;
        var rqt = new GenericRequest()
        {
            Messege = "Content12345678"
        };
        var result = false;
        AsyncNetworkEngine<GenericRequest, GenericResponse>.SendRequest(gcptestendpoint, rqt,
            (asyncResult, resp, error) =>
            {
                Assert.AreEqual(asyncResult, AsyncNetworkResult.SUCCESS);
                Assert.IsNull(error);
                Assert.AreEqual("Received:" + rqt.Messege, resp.Messege);
                result = true;
            });
        while (!result) yield return null;
    }
}

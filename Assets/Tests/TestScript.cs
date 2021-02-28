using System.Collections;
using UnityEngine.TestTools;
using Asyncnetworkengine;
using UnityEngine.Assertions;

public class TestScript
{
    private const string gcptestendpoint = "https://us-central1-game-workstore.cloudfunctions.net/gcptest";

    [UnityTest]
    public IEnumerator TestGCP()
    {

        AsyncNetworkEngine<GenericRequest, GenericResponse, GenericErrorResponse>.Cloud = AsyncNetworkEngineCloud.GCP;
        var rqt = new GenericRequest()
        {
            Messege = "Content12345678"
        };
        var result = false;
        AsyncNetworkEngine<GenericRequest, GenericResponse, GenericErrorResponse>.SendRequest(gcptestendpoint, rqt,
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

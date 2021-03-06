using Google.Protobuf;
using GameWorkstore.Patterns;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

namespace GameWorkstore.AsyncNetworkEngine
{
    public enum AsyncNetworkResult
    {
        NONE = 0,
        SUCCESS = 200,
        E_NETWORK,
        E_PROCESS,
        E_DATA_NULL,
        E_DATA_EMPTY,
        E_PARSER,
    }

    public enum AsyncNetworkEngineCloud
    {
        GCP = 0,
        AWS = 1
    }

    /// <summary>
    /// Implements a UnityRequest for google protobuf web functions.
    /// </summary>
    /// <typeparam name="TR">Request</typeparam>
    /// <typeparam name="TU">Response</typeparam>
    public static class AsyncNetworkEngine<TR, TU>
        where TR : IMessage<TR>, new()
        where TU : IMessage<TU>, new()
    {
        public static AsyncNetworkEngineCloud Cloud = AsyncNetworkEngineCloud.AWS;
        private static readonly MessageParser<TU> _tuParser = new MessageParser<TU>(() => new TU());
        private static readonly MessageParser<GenericErrorResponse> _tvParser = new MessageParser<GenericErrorResponse>(() => new GenericErrorResponse());
        private static EventService _eventService = null;

        public static void Send(string uri, TR request, Action<AsyncNetworkResult, TU, GenericErrorResponse> result)
        {
            if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
            _eventService.StartCoroutine(SendRequest(uri, request, result));
        }

        public static IEnumerator SendRequest(string uri, TR request, Action<AsyncNetworkResult, TU, GenericErrorResponse> result)
        {
            //Notice: APIGateway automatically converts binary data into base64 strings
            using var rqt = new UnityWebRequest(uri, "POST")
            {
                uploadHandler = new UploadHandlerRaw(request.ToByteArray()),
                downloadHandler = new DownloadHandlerBuffer()
            };
            yield return rqt.SendWebRequest();

            switch (rqt.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                    Return(AsyncNetworkResult.E_NETWORK, result);
                    break;
                    /*while (!rqt.downloadHandler.isDone) yield return null;
                    if (rqt.downloadHandler.data == null) { Return(AsyncNetworkResult.E_DATA_NULL, result); yield break; }
                    if (rqt.downloadHandler.data.Length <= 0) { Return(AsyncNetworkResult.E_DATA_EMPTY, result); yield break; }
                    try
                    {
                        if (Cloud == AsyncNetworkEngineCloud.AWS)
                        {
                            var data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
                            var error = _tvParser.ParseFrom(data);
                            Return(AsyncNetworkResult.E_PROCESS, default, error, result);
                        }
                        else
                        {
                            var error = _tvParser.ParseFrom(rqt.downloadHandler.data);
                            Return(AsyncNetworkResult.E_PROCESS, default, error, result);
                        }
                    }
                    catch
                    {
                        Return(AsyncNetworkResult.E_PROCESS, result);
                    }*/
                    break;
                case UnityWebRequest.Result.Success:
                    while (!rqt.downloadHandler.isDone) yield return null;
                    if (rqt.downloadHandler.data == null) { Return(AsyncNetworkResult.E_DATA_NULL, result); yield break; }
                    if (rqt.downloadHandler.data.Length <= 0) { Return(AsyncNetworkResult.E_DATA_EMPTY, result); yield break; }
                    try
                    {
                        if (Cloud == AsyncNetworkEngineCloud.AWS)
                        {
                            var data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
                            var packet = _tuParser.ParseFrom(data);
                            Return(AsyncNetworkResult.SUCCESS, packet, default, result);
                        }
                        else
                        {
                            var packet = _tuParser.ParseFrom(rqt.downloadHandler.data);
                            Return(AsyncNetworkResult.SUCCESS, packet, default, result);
                        }
                    }
                    catch
                    {
                        Return(AsyncNetworkResult.E_PARSER, result);
                    }
                    break;
            }
        }

        private static void Return(AsyncNetworkResult result, Action<AsyncNetworkResult, TU, GenericErrorResponse> callback)
        {
            Return(result, default, default, callback);
        }

        private static void Return(AsyncNetworkResult result, TU data, GenericErrorResponse error, Action<AsyncNetworkResult, TU, GenericErrorResponse> callback)
        {
            if (callback == null) return;
            _eventService.QueueAction(() => callback.Invoke(result, data, error));
        }
    }
}
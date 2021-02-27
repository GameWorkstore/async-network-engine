using Google.Protobuf;
using GameWorkstore.Patterns;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public enum AsyncNetworkResult
{
    SUCCESS,
    E_HTTP,
    E_NETWORK,
    E_DATA_NULL,
    E_DATA_EMPTY,
    E_PARSER
}

public enum AsyncNetworkEngineCloud
{
    GCP = 0,
    AWS = 1
}

/// <summary>
/// Implements a UnityRequest for google protobuf web functions.
/// </summary>
/// <typeparam name="T">Request</typeparam>
/// <typeparam name="TU">Response</typeparam>
/// <typeparam name="TV">GenericError</typeparam>
public static class AsyncNetworkEngine<T, TU, TV>
    where T : IMessage<T>, new()
    where TU : IMessage<TU>, new()
    where TV : IMessage<TV>, new()
{
    public static AsyncNetworkEngineCloud Cloud = AsyncNetworkEngineCloud.AWS;
    private static readonly MessageParser<TU> _tuParser = new MessageParser<TU>(() => new TU());
    private static readonly MessageParser<TV> _tvParser = new MessageParser<TV>(() => new TV());
    private static EventService _eventService = null;

    public static void Send(string uri, T request, Action<AsyncNetworkResult, TU, TV> result)
    {
        if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
        _eventService.StartCoroutine(SendRequest(uri, request, result));
    }

    private static IEnumerator SendRequest(string uri, T request, Action<AsyncNetworkResult, TU, TV> result)
    {
        //Notice: APIGateway automatically converts binary data into base64 strings
        using var rqt = new UnityWebRequest(uri, "POST")
        {
            uploadHandler = new UploadHandlerRaw(request.ToByteArray()),
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return rqt.SendWebRequest();
        if (rqt.result != UnityWebRequest.Result.Success) { Return(AsyncNetworkResult.E_NETWORK, result); yield break; }
        
        while (!rqt.downloadHandler.isDone) yield return null;
        if (rqt.downloadHandler.data == null) { Return(AsyncNetworkResult.E_DATA_NULL, result); yield break; }
        if (rqt.downloadHandler.data.Length <= 0) { Return(AsyncNetworkResult.E_DATA_EMPTY, result); yield break; }
        if (rqt.responseCode != 200)
        {
            if (rqt.downloadHandler.data == null) { Return(AsyncNetworkResult.E_DATA_NULL, result); yield break; }
            if (rqt.downloadHandler.data.Length <= 0) { Return(AsyncNetworkResult.E_DATA_EMPTY, result); yield break; }
            try
            {
                var data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
                var error = _tvParser.ParseFrom(data);
                Return(AsyncNetworkResult.E_HTTP, default, error, result);
            }
            catch
            {
                Return(AsyncNetworkResult.E_HTTP, result);
            }
            yield break;
        }
        
        try
        {
            var data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
            var packet = _tuParser.ParseFrom(data);
            Return(AsyncNetworkResult.SUCCESS, packet, default, result);
        }
        catch
        {
            Return(AsyncNetworkResult.E_PARSER, result);
        }
    }

    private static void Return(AsyncNetworkResult result, Action<AsyncNetworkResult, TU, TV> callback)
    {
        Return(result, default, default, callback);
    }

    private static void Return(AsyncNetworkResult result, TU data, TV error, Action<AsyncNetworkResult, TU, TV> callback)
    {
        if (callback == null) return;
        _eventService.QueueAction(() => callback.Invoke(result, data, error));
    }
}
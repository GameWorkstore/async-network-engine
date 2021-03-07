using Google.Protobuf;
using GameWorkstore.Patterns;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace GameWorkstore.AsyncNetworkEngine
{
    public enum CloudProvider
    {
        GCP = 0,
        AWS = 1
    }

    public static class AsyncNetworkEngineMap
    {
        internal static bool IsSingleCloud = true;
        internal static CloudProvider SingleCloudProvider = CloudProvider.AWS;
        internal static Dictionary<string, CloudProvider> MapCloudProvider = null;

        /// <summary>
        /// Setup a single cloud provider for all functions.
        /// </summary>
        /// <param name="cloudProvider">Target cloud provider implementation.</param>
        public static void SetupCloud(CloudProvider cloudProvider)
        {
            IsSingleCloud = true;
            SingleCloudProvider = cloudProvider;
        }

        /// <summary>
        /// Setup a multi cloud provider for all functions.
        /// </summary>
        /// <param name="mapCloudProvider">Maps base url to cloud provider. Use the lowest possible string to differentiate clouds.</param>
        public static void SetupCloudMap(Dictionary<string, CloudProvider> mapCloudProvider)
        {
            IsSingleCloud = false;
            MapCloudProvider = mapCloudProvider;
        }
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
        private static readonly MessageParser<TU> _tuParser = new MessageParser<TU>(() => new TU());
        private static readonly MessageParser<GenericErrorResponse> _tvParser = new MessageParser<GenericErrorResponse>(() => new GenericErrorResponse());
        private static EventService _eventService = null;

        public static void Send(string uri, TR request, Action<Transmission, TU, GenericErrorResponse> result)
        {
            if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
            _eventService.StartCoroutine(SendRequest(uri, request, result));
        }

        public static IEnumerator SendRequest(string uri, TR request, Action<Transmission, TU, GenericErrorResponse> result)
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
                    Return(Transmission.ErrorConnection, result);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    HandleError(GetCloudProvider(ref uri), rqt, result);
                    break;
                case UnityWebRequest.Result.Success:
                    while (!rqt.downloadHandler.isDone) yield return null;
                    HandleSuccess(GetCloudProvider(ref uri), rqt, result);
                    break;
            }
        }

        private static CloudProvider GetCloudProvider(ref string url)
        {
            if (!AsyncNetworkEngineMap.IsSingleCloud)
            {
                foreach (var pair in AsyncNetworkEngineMap.MapCloudProvider)
                {
                    if (!url.StartsWith(pair.Key)) continue;
                    return pair.Value;
                }
            }
            return AsyncNetworkEngineMap.SingleCloudProvider;
        }

        private static void HandleSuccess(CloudProvider provider, UnityWebRequest rqt, Action<Transmission, TU, GenericErrorResponse> result)
        {
            if (rqt.downloadHandler.data == null)
            {
                Return(Transmission.ErrorNoData, result);
                return;
            }
            if (rqt.downloadHandler.data.Length <= 0)
            {
                Return(Transmission.ErrorNoData, result);
                return;
            }

            byte[] data = rqt.downloadHandler.data;
            if (provider == CloudProvider.AWS)
            {
                data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
            }

            try
            {
                var packet = _tuParser.ParseFrom(data);
                Return(Transmission.Success, packet, default, result);
            }
            catch
            {
                Return(Transmission.ErrorParser, result);
            }
        }

        private static void HandleError(CloudProvider provider, UnityWebRequest rqt, Action<Transmission, TU, GenericErrorResponse> result)
        {
            if (rqt.downloadHandler.data == null)
            {
                Return(Transmission.ErrorProtocol, result);
                return;
            }
            if (rqt.downloadHandler.data.Length <= 0)
            {
                Return(Transmission.ErrorProtocol, result);
                return;
            }

            byte[] data = rqt.downloadHandler.data;
            if (provider == CloudProvider.AWS)
            {
                data = Base64Url.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
            }

            var transmission = (Transmission)rqt.responseCode;
            try
            {
                var packet = _tvParser.ParseFrom(data);
                Return(transmission, default, packet, result);
            }
            catch
            {
                Return(Transmission.ErrorParser, result);
            }
        }

        private static void Return(Transmission result, Action<Transmission, TU, GenericErrorResponse> callback)
        {
            Return(result, default, default, callback);
        }

        private static void Return(Transmission result, TU data, GenericErrorResponse error, Action<Transmission, TU, GenericErrorResponse> callback)
        {
            if (callback == null) return;
            //_eventService.QueueAction(() => callback.Invoke(result, data, error));
            callback.Invoke(result, data, error);
        }
    }
}
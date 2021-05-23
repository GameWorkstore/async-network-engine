using Google.Protobuf;
using GameWorkstore.Patterns;
using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable StaticMemberInGenericType

namespace GameWorkstore.AsyncNetworkEngine
{
    public enum CloudProvider
    {
        Gcp = 0,
        Aws = 1
    }

    public class FileData
    {
        public string URL;
        public byte[] Data;
    }

    public static class AsyncNetworkEngineMap
    {
        internal static bool IsSingleCloud = true;
        internal static CloudProvider SingleCloudProvider = CloudProvider.Aws;
        internal static Dictionary<string, CloudProvider> MapCloudProvider;

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

    public static class AsyncNetworkEngine
    {
        private static EventService _eventService;

        public static void Download(string url,Action<Transmission,FileData> callback)
        {
            if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
            _eventService.StartCoroutine(SendRequest(new[] { url }, (result,files) => {
                callback?.Invoke(result,files.FirstOrDefault());
            }));
        }

        public static void Download(string[] urls, Action<Transmission,HighSpeedArray<FileData>> callback)
        {
            if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
            _eventService.StartCoroutine(SendRequest(urls, callback));
        }

        public static IEnumerator SendRequest(string[] urls, Action<Transmission, HighSpeedArray<FileData>> callback)
        {
            var data = new HighSpeedArray<FileData>(urls.Length);
            foreach(var url in urls)
            {
                using var rqt = UnityWebRequest.Get(url);
                yield return rqt.SendWebRequest();

                switch(rqt.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Return(Transmission.ErrorConnection, null, callback);
                        break;
                    case UnityWebRequest.Result.Success:
                        data.Add(new FileData()
                        {
                            URL = url,
                            Data = rqt.downloadHandler.data
                        });
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Return(Transmission.ErrorProtocol, null, callback);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Return(Transmission.ErrorDecode, null, callback);
                        break;
                }
            }
            Return(Transmission.Success, data, callback);
        }

        private static void Return(Transmission result, HighSpeedArray<FileData> data, Action<Transmission, HighSpeedArray<FileData>> callback)
        {
            callback?.Invoke(result,data);
        }
    }

    /// <summary>
    /// Implements a UnityRequest for google protobuf web functions.
    /// </summary>
    /// <typeparam name="TRqt">Request</typeparam>
    /// <typeparam name="TResp">Response</typeparam>
    public static class AsyncNetworkEngine<TRqt,TResp>
        where TRqt : IMessage<TRqt>, new()
        where TResp : IMessage<TResp>, new()
    {
        private static readonly MessageParser<TResp> _tuParser = new MessageParser<TResp>(() => new TResp());
        private static readonly MessageParser<GenericErrorResponse> _tvParser = new MessageParser<GenericErrorResponse>(() => new GenericErrorResponse());
        private static EventService _eventService;

        public static void Send(string url, TRqt request, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            if (_eventService == null) _eventService = ServiceProvider.GetService<EventService>();
            _eventService.StartCoroutine(SendRequest(url, request, callback));
        }

        public static IEnumerator SendRequest(string url, TRqt request, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            //Notice: APIGateway automatically converts binary data into base64 strings
            using var rqt = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(request.ToByteArray()),
                downloadHandler = new DownloadHandlerBuffer()
            };
            yield return rqt.SendWebRequest();

            switch (rqt.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Return(Transmission.ErrorConnection, callback);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    HandleError(GetCloudProvider(ref url), rqt, callback);
                    break;
                case UnityWebRequest.Result.Success:
                    while (!rqt.downloadHandler.isDone) yield return null;
                    HandleSuccess(GetCloudProvider(ref url), rqt, callback);
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

        private static void HandleSuccess(CloudProvider provider, UnityWebRequest rqt, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            if (rqt.downloadHandler.data == null)
            {
                Return(Transmission.ErrorNoData, callback);
                return;
            }

            var data = rqt.downloadHandler.data;
            if (provider == CloudProvider.Aws)
            {
                data = Base64StdEncoding.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
            }

            TResp packet;
            try
            {
                packet = _tuParser.ParseFrom(data);
            }
            catch
            {
                packet = default;
            }
            Return(packet != null ? Transmission.Success : Transmission.ErrorParser, packet, default, callback);
        }

        private static void HandleError(CloudProvider provider, UnityWebRequest rqt, Action<Transmission, TResp, GenericErrorResponse> result)
        {
            if (rqt.downloadHandler.data == null)
            {
                Return(Transmission.ErrorProtocol, result);
                return;
            }

            var data = rqt.downloadHandler.data;
            if (provider == CloudProvider.Aws)
            {
                data = Base64StdEncoding.Decode(Encoding.ASCII.GetString(rqt.downloadHandler.data));
            }

            var transmission = (Transmission)rqt.responseCode;
            GenericErrorResponse packet;
            try
            {
                packet = _tvParser.ParseFrom(data);
            }
            catch
            {
                packet = default;
            }
            Return(packet != null ? transmission : Transmission.ErrorParser, default, packet, result);
        }

        private static void Return(Transmission result, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            Return(result, default, default, callback);
        }

        private static void Return(Transmission result, TResp data, GenericErrorResponse error, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            if (callback == null) return;
            _eventService.QueueAction(() => callback.Invoke(result, data, error));
            //callback.Invoke(result, data, error);
        }
    }
}
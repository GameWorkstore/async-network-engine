﻿using Google.Protobuf;
using System;
using System.Text;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameWorkstore.AsyncNetworkEngine
{
    public enum CloudProvider
    {
        Gcp = 0,
        Aws = 1
    }

    public class FileData
    {
        public Transmission Result;
        public string URL;
        public byte[] Data;
    }

    public class ProgressData
    {
        public FileData[] Files;
        public float Progress => Length > 0 ? (float)Downloaded / Length : 0;
        private int Downloaded => Files.Count(IsDownloaded);

        private int Failed => Files.Count(IsFailed);

        public int Length => Files.Length;

        private bool IsDownloaded(FileData file)
        {
            return file.Result == Transmission.Success;
        }

        private bool IsFailed(FileData file)
        {
            return file.Result != Transmission.Success && file.Result != Transmission.NotSpecified;
        }

        public override string ToString()
        {
            return $"ProgressData: Progress {Progress} Downloaded {Downloaded} Failed {Failed} Total {Length}";
        }
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
        public static void Download(
            string url,
            Action<FileData> callback
        )
        {
            Send(new[] { url },
                (files) => callback?.Invoke(files.Files.FirstOrDefault()),
                null
            );
        }

        public static void Download(
            string[] urls,
            Action<ProgressData> onDownloadFinished
        )
        {
            Download(urls, onDownloadFinished, null);
        }

        public static void Download(
            string[] urls,
            Action<ProgressData> onDownloadFinished,
            Action<ProgressData> onDownloadInProgress)
        {
            Send(urls, onDownloadFinished, onDownloadInProgress);
        }

        public static void Send(
            string[] urls,
            Action<ProgressData> onDownloadFinished,
            Action<ProgressData> onDownloadInProgress
        )
        {
            var progress = new ProgressData()
            {
                Files = new FileData[urls.Length]
            };
            for (int i = 0; i < urls.Length; i++)
            {
                progress.Files[i] = new FileData()
                {
                    Result = Transmission.NotSpecified,
                    URL = urls[i]
                };
            }
            for (int i = 0; i < progress.Length; i++)
            {
                var file = progress.Files[i];
                var rqt = UnityWebRequest.Get(file.URL);
                rqt.SendWebRequest().completed += (op) =>
                {
                    switch (rqt.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                            file.Result = Transmission.ErrorConnection;
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            file.Result = Transmission.ErrorProtocol;
                            break;
                        case UnityWebRequest.Result.DataProcessingError:
                            file.Result = Transmission.ErrorDecode;
                            break;
                        case UnityWebRequest.Result.Success:
                            file.Result = Transmission.Success;
                            file.Data = rqt.downloadHandler.data;
                            break;
                        default:
                            file.Result = Transmission.ErrorProtocol;
                            break;
                    }
                    rqt.Dispose();
                    Return(progress, onDownloadFinished, onDownloadInProgress);
                };
            }
        }

        private static bool NotSpecified(FileData file)
        {
            return file.Result == Transmission.NotSpecified;
        }

        private static void Return(
            ProgressData progressData, Action<ProgressData> callbackFinal, Action<ProgressData> callback
        ){
            callback?.Invoke(progressData);
            if (progressData.Files.Any(NotSpecified)) return;
            callbackFinal?.Invoke(progressData);
        }
    }

    /// <summary>
    /// Implements a UnityRequest for google protobuf web functions.
    /// </summary>
    /// <typeparam name="TRqt">Request</typeparam>
    /// <typeparam name="TResp">Response</typeparam>
    public static class AsyncNetworkEngine<TRqt, TResp>
        where TRqt : IMessage<TRqt>, new()
        where TResp : IMessage<TResp>, new()
    {
        private static readonly MessageParser<TResp> _tuParser = new MessageParser<TResp>(() => new TResp());
        private static readonly MessageParser<GenericErrorResponse> _tvParser = new MessageParser<GenericErrorResponse>(() => new GenericErrorResponse());

        public static void Send(string url, TRqt request, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            //Notice: APIGateway automatically converts binary data into base64 strings
            var rqt = new UnityWebRequest(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(request.ToByteArray()),
                downloadHandler = new DownloadHandlerBuffer()
            };
            rqt.SendWebRequest().completed += _ =>
            {
                switch (rqt.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Return(Transmission.ErrorConnection, callback);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        HandleError(GetCloudProvider(ref url), rqt, callback);
                        break;
                    case UnityWebRequest.Result.Success:
                        while (!rqt.downloadHandler.isDone);
                        HandleSuccess(GetCloudProvider(ref url), rqt, callback);
                        break;
                }
                rqt.Dispose();
            };
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
                var s = Encoding.ASCII.GetString(rqt.downloadHandler.data);
                if (!Base64StdEncoding.Decode(s, out data))
                {
                    Return(Transmission.ErrorParser, default, new GenericErrorResponse() { Error = "base64 string is invalid:" + s }, callback);
                    return;
                }
            }

            TResp packet;
            try
            {
                packet = _tuParser.ParseFrom(data);
            }
            catch
            {
                Return(Transmission.ErrorParser, callback);
                return;
            }
            Return(Transmission.Success, packet, default, callback);
        }

        private static void HandleError(CloudProvider provider, UnityWebRequest rqt, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            if (rqt.downloadHandler.data == null)
            {
                Return(Transmission.ErrorProtocol, callback);
                return;
            }

            var data = rqt.downloadHandler.data;
            if (provider == CloudProvider.Aws)
            {
                var s = Encoding.ASCII.GetString(rqt.downloadHandler.data);
                if (!Base64StdEncoding.Decode(s, out data))
                {
                    Return(Transmission.ErrorParser, default, new GenericErrorResponse() { Error = "base64 string is invalid:" + s }, callback);
                    return;
                }
            }

            var transmission = (Transmission)rqt.responseCode;
            GenericErrorResponse packet;
            try
            {
                packet = _tvParser.ParseFrom(data);
            }
            catch
            {
                Return(Transmission.ErrorParser, callback);
                return;
            }
            Return(transmission, default, packet, callback);
        }

        private static void Return(Transmission result, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            Return(result, default, default, callback);
        }

        private static void Return(Transmission result, TResp data, GenericErrorResponse error, Action<Transmission, TResp, GenericErrorResponse> callback)
        {
            callback?.Invoke(result, data, error);
        }
    }
}
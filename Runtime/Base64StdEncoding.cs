using System;
using UnityEngine;

namespace GameWorkstore.AsyncNetworkEngine
{

    public static class Base64StdEncoding
    {
        public static string Encode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            //output = output.Replace('+', '-'); // 62nd char of encoding
            //output = output.Replace('/', '_'); // 63rd char of encoding
            output = output.Split('=')[0]; // Remove any trailing '='s

            return output;
        }
        
        public static bool Decode(string input, out byte[] data)
        {
            data = null;
            switch (input.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    input += "==";
                    break; // Two pad chars
                case 3:
                    input += "=";
                    break; // One pad char
                default:
                    return false;
            }
            try
            {
                data = Convert.FromBase64String(input);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
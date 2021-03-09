using System;

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

        public static byte[] Decode(string input)
        {
            //var output = input;
            //output = output.Replace('-', '+'); // 62nd char of encoding
            //output = output.Replace('_', '/'); // 63rd char of encoding

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
                    throw new ArgumentOutOfRangeException(nameof(input), "Illegal base64url string!");
            }

            return Convert.FromBase64String(input);
        }
    }
}
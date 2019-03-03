using System;
using System.Diagnostics;
using Cloudmersive.APIClient.NET.OCR.Api;
using Cloudmersive.APIClient.NET.OCR.Client;
using Cloudmersive.APIClient.NET.OCR.Model;

namespace TraCuuThongBaoPhatHanh.Service
{
    public class CloudMersive : IBaseService
    {
        private static readonly ImageOcrApi ApiInstance = new ImageOcrApi();
        private readonly string[] _keys;
        private static readonly Random Random = new Random();

        public CloudMersive(string apiKeys)
        {
            _keys = string.IsNullOrWhiteSpace(apiKeys)
                ? new[] {"35b6b7f0-64be-49a6-9e78-f237c4e04975"}
                : apiKeys.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public string ImageOcrToText(string imagePath)
        {
            string apiValue = _keys[Random.Next(_keys.Length)];
            Configuration.Default.AddApiKey("Apikey", apiValue);
            var imageFile = new System.IO.FileStream(imagePath, System.IO.FileMode.Open);
            try
            {
                // Converts an uploaded image in common formats such as JPEG, PNG into text via Optical Character Recognition.
                ImageToTextResponse result = ApiInstance.ImageOcrPost(imageFile, "ENG");
                Debug.WriteLine(result);
                return result?.TextResult;
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ImageOcrApi.ImageOcrPost: " + e.Message);
                throw;
            }
        }
    }
}

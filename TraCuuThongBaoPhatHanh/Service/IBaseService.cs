using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TraCuuThongBaoPhatHanh.Service
{
    public interface IBaseService
    {
        string ImageOcrToText(string imagePath);
    }

    public class ServiceHelper
    {
        private static readonly string ApiKeys;
        static ServiceHelper()
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //T
            using (var webClient = new WebClient())
            {
                string json = webClient.DownloadString("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/TraCuuThongBaoPhatHanh/API.json");
                JObject jObject = JObject.Parse(json);
                ApiKeys = (string)jObject["CloudMersive"];
            }
        }
        public static IBaseService GetService()
        {
            string service = ConfigurationManager.AppSettings["service"];
            if (string.IsNullOrWhiteSpace(service) || service == "0")
            {
                return new TesseractService();
            }

            return new CloudMersive(ApiKeys);
        }

        public static string GetDesiredFileFullPath(string destinationDirectory, string fileFullPath, string remark = null)
        {
            string fileName = Path.GetFileNameWithoutExtension(fileFullPath);
            string extension = Path.GetExtension(fileFullPath);

            var desiredFileFullPath = Path.Combine(destinationDirectory, $"{fileName}{remark}{extension}");

            int count = 1;
            while (File.Exists(desiredFileFullPath))
            {
                desiredFileFullPath = Path.Combine(destinationDirectory, $"{fileName}{remark} ({count}){extension}");
                ++count;
            }

            return desiredFileFullPath;
        }
    }
}
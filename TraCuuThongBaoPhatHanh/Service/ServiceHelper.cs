using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace TraCuuThongBaoPhatHanh.Service
{
    public class ServiceHelper
    {
        private static readonly string ApiKeys;
        static ServiceHelper()
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //T
            using (var webClient = new WebClient())
            {
                string json = webClient.DownloadString("https://raw.githubusercontent.com/Devancy/TraCuuThongBaoPhatHanh/master/TraCuuThongBaoPhatHanh/Deploy/API.json");
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

        public void DebugToggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exeConfig = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".config";
            if (System.IO.File.Exists(exeConfig))
            {
                XmlDocument config = new XmlDocument();
                config.Load(exeConfig);
                var node = config.SelectSingleNode("/configuration/appSettings[@key='service']");
                if (node?.Attributes != null)
                {
                    string value = node.Attributes["value"].Value != "1" ? "1" : "0";
                    node.Attributes["value"].Value = value;
                    MessageBox.Show($"Đã thiết lập logger level {value} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var appSettings = config.SelectSingleNode("/configuration/appSettings");
                    XmlNode kvp = config.CreateElement("add");
                    XmlAttribute addAttribute = config.CreateAttribute("key");
                    addAttribute.Value = "service";
                    kvp.Attributes.Append(addAttribute);
                    addAttribute = config.CreateAttribute("value");
                    addAttribute.Value = "1";
                    kvp.Attributes.Append(addAttribute);
                    MessageBox.Show("File logging config không tìm thấy /configuration/log4net[@debug='true']/logger[@name='EInvoice.Shared']/level", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                config.Save(exeConfig);
            }
            else
            {
                MessageBox.Show("File log không tồn tại", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
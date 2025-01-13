using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServerWebApplication.Models;
using TemporaryServer.Models;

namespace ServerWebApplication.Controllers
{
    public class UserInfo
    {
        public readonly int userId;
        public readonly string name;

        public UserInfo(int userId, string name)
        {
            this.userId = userId;
            this.name = name;
        }
    }

    public class HomeController : Controller
    {

        [HttpPost]
        public async Task<IActionResult> Index()
        {

            // 빈값 에러 처리
            if (Request.ContentLength == 0)
            {
                SendPacketBase packet = new SendPacketBase(PACKET_NAME_TYPE.None, RETURN_CODE.Error);
                string sendData = JsonConvert.SerializeObject(packet);
                return Content(sendData);
            }

            string json = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            ReceivePacketBase? receivePacketBase = JsonConvert.DeserializeObject<ReceivePacketBase>(json);

            SendPacketBase sendPacketBase = null;
            if (receivePacketBase != null && Enum.TryParse(receivePacketBase.PacketName, out PACKET_NAME_TYPE type))
            {
                switch (type)
                {
                    case PACKET_NAME_TYPE.ApplicationConfig:
                        sendPacketBase = ApplicationConfig(json);
                        break;
                    case PACKET_NAME_TYPE.Maintenance:
                        sendPacketBase = Maintenance(json);
                        break;
                    default:
                        {
                            SendPacketBase packet = new SendPacketBase(PACKET_NAME_TYPE.None, RETURN_CODE.Error);
                            string sendData = JsonConvert.SerializeObject(packet);
                            return Content(sendData);
                        }
                }
            }
            else
            {
                SendPacketBase packet = new SendPacketBase(PACKET_NAME_TYPE.None, RETURN_CODE.Error);
                string sendData = JsonConvert.SerializeObject(packet);
                return Content(sendData);
            }

            return Content(JsonConvert.SerializeObject(sendPacketBase));
        }

        private SendPacketBase Maintenance(string json)
        {
            MaintenanceReceivePacket? receivePacket = JsonConvert.DeserializeObject<MaintenanceReceivePacket>(json);

            if (receivePacket == null)
            {
                return new SendPacketBase(PACKET_NAME_TYPE.Maintenance, RETURN_CODE.Error);
            }

            bool isMaintenance = false;
            if(CompareVersions(receivePacket.AppVersion, "1.0.0") <= 0)
            {
                isMaintenance = true;
            }

            string title = string.Empty;
            string contents = string.Empty;
            if ((LANGUAGE_TYPE)receivePacket.languageType == LANGUAGE_TYPE.English)
            {
                title = "Maintenance";
                contents = "Regular inspection is underway. (AM 11:00 ~ PM 01:00)";
            }
            else if ((LANGUAGE_TYPE)receivePacket.languageType == LANGUAGE_TYPE.Korean)
            {
                title = "점검";
                contents = "정기점검중이다. (AM 11:00 ~ PM 01:00)";
            }


            PACKET_NAME_TYPE packetNameType = PACKET_NAME_TYPE.None;
            if (Enum.TryParse(receivePacket.PacketName, out PACKET_NAME_TYPE type))
            {
                packetNameType = type;
            }

            MaintenanceSendPacket sendPacket = new MaintenanceSendPacket(packetNameType, RETURN_CODE.Success, isMaintenance, title, contents);

            return sendPacket;
        }

        private int CompareVersions(string version1, string version2)
        {
            string[] parts1 = version1.Split('.');
            string[] parts2 = version2.Split('.');

            for (int i = 0; i < Math.Max(parts1.Length, parts2.Length); i++)
            {
                int num1 = (i < parts1.Length) ? int.Parse(parts1[i]) : 0;
                int num2 = (i < parts2.Length) ? int.Parse(parts2[i]) : 0;

                if (num1 < num2)
                {
                    return -1;
                }
                else if (num1 > num2)
                {
                    return 1;
                }
            }

            return 0; // versions are equal
        }
            
        private SendPacketBase ApplicationConfig(string json)
        {
            ApplicationConfigReceivePacket? receivePacket = JsonConvert.DeserializeObject<ApplicationConfigReceivePacket>(json);

            if (receivePacket == null)
            {
                return new SendPacketBase(PACKET_NAME_TYPE.ApplicationConfig, RETURN_CODE.Error);
            }

            // Api Url
            string apiUrl = string.Empty;
            switch ((ENVIRONMENT_TYPE)receivePacket.E_ENVIRONMENT_TYPE)
            {
                case ENVIRONMENT_TYPE.Dev:
                    apiUrl = "https://localhost:5000";
                    break;
                case ENVIRONMENT_TYPE.Stage:
                    apiUrl = "https://localhost:5000";
                    break;
                case ENVIRONMENT_TYPE.Live:
                    apiUrl = "https://localhost:5000";
                    break;
                default:
                    apiUrl = "https://localhost:5000";
                    break;
            }

            // 권한 설정
            DEVELOPMENT_ID_AUTHORITY dEVELOPMENT_ID_AUTHORITY = DEVELOPMENT_ID_AUTHORITY.None;
            if(receivePacket.DevelopmentId == "test123")
            {
                dEVELOPMENT_ID_AUTHORITY = DEVELOPMENT_ID_AUTHORITY.Tester;
            }

            PACKET_NAME_TYPE packetNameType = PACKET_NAME_TYPE.None;
            if (Enum.TryParse(receivePacket.PacketName, out PACKET_NAME_TYPE type))
            {
                packetNameType = type;
            }

            ApplicationConfigSendPacket applicationConfigSendPacket = new ApplicationConfigSendPacket(packetNameType, RETURN_CODE.Success, apiUrl, dEVELOPMENT_ID_AUTHORITY);

            return applicationConfigSendPacket;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

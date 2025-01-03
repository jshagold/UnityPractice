using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TemporaryServer.Models;

namespace TemporaryServer.Controllers
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

    /*
     [Server]
     */
    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index()
        {

            // 빈값 에러 처리
            if(Request.ContentLength == 0)
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
            if(receivePacketBase != null && Enum.TryParse(receivePacketBase.PacketName, out PACKET_NAME_TYPE type))
            {
                switch(type)
                {
                    case PACKET_NAME_TYPE.ApplicationConfig:
                        sendPacketBase = ApplicationConfig(json);
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

        private SendPacketBase ApplicationConfig(string json)
        {
            ApplicationConfigReceivePacket? applicationConfigReceivePacket = JsonConvert.DeserializeObject<ApplicationConfigReceivePacket>(json);

            if(applicationConfigReceivePacket == null)
            {
                return new SendPacketBase(PACKET_NAME_TYPE.None, RETURN_CODE.Error);
            }

            string apiUrl = string.Empty;
            switch ((ENVIRONMENT_TYPE)applicationConfigReceivePacket.E_ENVIRONMENT_TYPE)
            {
                case ENVIRONMENT_TYPE.Dev:
                    apiUrl = "https://localhost:8080";
                    break;
                case ENVIRONMENT_TYPE.Stage:
                    apiUrl = "https://localhost:8080";
                    break;
                case ENVIRONMENT_TYPE.Live:
                    apiUrl = "https://localhost:8080";
                    break;
                default:
                    apiUrl = "https://localhost:8080";
                    break;
            }


            PACKET_NAME_TYPE packetNameType = PACKET_NAME_TYPE.None;
            if (Enum.TryParse(applicationConfigReceivePacket.PacketName, out PACKET_NAME_TYPE type))
            {
                packetNameType = type;
            }

            ApplicationConfigSendPacket applicationConfigSendPacket = new ApplicationConfigSendPacket(packetNameType, RETURN_CODE.Success, apiUrl);

            return applicationConfigSendPacket;
        }

    }
}

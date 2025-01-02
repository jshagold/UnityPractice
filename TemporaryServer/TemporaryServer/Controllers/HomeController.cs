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

    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            string json = string.Empty;

            using (var reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            ApplicationConfigReceivePacket? applicationConfigReceivePacket = JsonConvert.DeserializeObject<ApplicationConfigReceivePacket>(json);

            ApplicationConfigSendPacket applicationConfigSendPacket = new ApplicationConfigSendPacket(applicationConfigReceivePacket.PacketName, 200, "https://loaclhost:8080/");

            string packet = JsonConvert.SerializeObject(applicationConfigSendPacket);
            return Content(packet);
        }

    }
}

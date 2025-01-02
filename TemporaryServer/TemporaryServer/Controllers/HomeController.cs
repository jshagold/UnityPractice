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
        public IActionResult Index()
        {
            string json = string.Empty;

            using (var reader = new StreamReader(Request.Body))
            {
                reader.ReadToEnd();
            }



            UserInfo userInfo = new UserInfo(3, "John");
            string packet = JsonConvert.SerializeObject(userInfo);

            return Content(packet);
        }

    }
}

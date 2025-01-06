using System.Runtime.InteropServices.JavaScript;

namespace TemporaryServer.Models
{
    public class ApplicationConfigReceivePacket : ReceivePacketBase
    {
        // environment - dev, stage, live
        // os type - Android, IOS
        // version - 1.0.0

        public int E_ENVIRONMENT_TYPE;
        public int E_OS_TYPE;
        public string AppVersion;
        public string DevelopmentId;

        public ApplicationConfigReceivePacket(string packetName, int e_ENVIRONMENT_TYPE, int e_OS_TYPE, string appVersion, string developmentId) : base(packetName)
        {
            this.E_ENVIRONMENT_TYPE = e_ENVIRONMENT_TYPE;
            this.E_OS_TYPE = e_OS_TYPE;
            this.AppVersion = appVersion;
            this.DevelopmentId = developmentId;
        }
    }

    public class ApplicationConfigSendPacket : SendPacketBase
    {
        public string ApiUrl;
        public int DevelopmentIdAuthority;

        public ApplicationConfigSendPacket(PACKET_NAME_TYPE packetName, RETURN_CODE returnCode, string apiUrl, DEVELOPMENT_ID_AUTHORITY developmentIdAuthority) : base(packetName, returnCode)
        {
            this.ApiUrl = apiUrl;
            this.DevelopmentIdAuthority = (int)developmentIdAuthority;
        }
    }
}

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

        public ApplicationConfigReceivePacket(string packetName, int e_ENVIRONMENT_TYPE, int e_OS_TYPE, string appVersion) : base(packetName)
        {
            E_ENVIRONMENT_TYPE = e_ENVIRONMENT_TYPE;
            E_OS_TYPE = e_OS_TYPE;
            AppVersion = appVersion;
        }
    }

    public class ApplicationConfigSendPacket : SendPacketBase
    {
        public readonly string ApiUrl;

        public ApplicationConfigSendPacket(string packetName, int returnCode, string apiUrl) : base(packetName, returnCode)
        {
            ApiUrl = apiUrl;
        }
    }
}

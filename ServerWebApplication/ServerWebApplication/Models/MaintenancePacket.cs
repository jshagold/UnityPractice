using TemporaryServer.Models;

namespace ServerWebApplication.Models
{
    public class MaintenanceReceivePacket : ReceivePacketBase
    {
        // environment - dev, stage, live
        // os type - Android, IOS
        // version - 1.0.0

        public int E_ENVIRONMENT_TYPE;
        public int E_OS_TYPE;
        public string AppVersion;
        public int languageType;

        public MaintenanceReceivePacket(string packetName, int e_ENVIRONMENT_TYPE, int e_OS_TYPE, string appVersion, int languageType) : base(packetName)
        {
            this.E_ENVIRONMENT_TYPE = e_ENVIRONMENT_TYPE;
            this.E_OS_TYPE = e_OS_TYPE;
            this.AppVersion = appVersion;
            this.languageType = languageType;
        }

    }

    public class MaintenanceSendPacket : SendPacketBase
    {
        public bool IsMaintenance;
        public string Title;
        public string Contents;

        public MaintenanceSendPacket(PACKET_NAME_TYPE packetName, RETURN_CODE returnCode, bool isMaintenance, string title, string contents) : base(packetName, returnCode)
        {
            this.IsMaintenance = isMaintenance;
            this.Title = title;
            this.Contents = contents;
        }
    }
}

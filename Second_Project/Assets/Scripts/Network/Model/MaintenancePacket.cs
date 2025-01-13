using UnityEngine;

public class MaintenanceSendPacket : SendPacketBase
{
    // environment - dev, stage, live
    // os type - Android, IOS
    // version - 1.0.0

    public int E_ENVIRONMENT_TYPE;
    public int E_OS_TYPE;
    public string AppVersion;
    public int languageType;

    public MaintenanceSendPacket(string url, PACKET_NAME_TYPE packetName, ENVIRONMENT_TYPE e_ENVIRONMENT_TYPE, OS_TYPE e_OS_TYPE, string appVersion, SystemLanguage languageType) : base(url, packetName)
    {
        this.E_ENVIRONMENT_TYPE = (int)e_ENVIRONMENT_TYPE;
        this.E_OS_TYPE = (int)e_OS_TYPE;
        this.AppVersion = appVersion;
        this.languageType = (int)languageType;
    }

}

public class MaintenanceReceivePacket : ReceivePacketBase
{
    public string ApiUrl;
    public int DevelopmentIdAuthority;
    public bool IsMaintenance;
    public string Title;
    public string Contents;

    public MaintenanceReceivePacket(int returnCode, string apiUrl, bool isMaintenance, string title, string contents) : base(returnCode)
    {
        this.ApiUrl = apiUrl;
        this.IsMaintenance = isMaintenance;
        this.Title = title;
        this.Contents = contents;
    }
}
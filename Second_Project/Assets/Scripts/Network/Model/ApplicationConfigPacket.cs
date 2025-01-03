public class ApplicationConfigSendPacket : SendPacketBase
{
    // environment - dev, stage, live
    // os type - Android, IOS
    // version - 1.0.0

    public int E_ENVIRONMENT_TYPE;
    public int E_OS_TYPE;
    public string AppVersion;

    public ApplicationConfigSendPacket(string url, PACKET_NAME_TYPE packetName, ENVIRONMENT_TYPE e_ENVIRONMENT_TYPE, OS_TYPE e_OS_TYPE, string appVersion) : base(url, packetName)
    {
        E_ENVIRONMENT_TYPE = (int)e_ENVIRONMENT_TYPE;
        E_OS_TYPE = (int)e_OS_TYPE;
        AppVersion = appVersion;
    }
}

public class ApplicationConfigReceivePacket : ReceivePacketBase
{
    public string ApiUrl;

    public ApplicationConfigReceivePacket(int returnCode, string apiUrl) : base(returnCode)
    {
        ApiUrl = apiUrl;
    }
}
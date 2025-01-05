public class SendPacketBase
{
    public string Url;
    public string PacketName;

    public SendPacketBase(string Url, PACKET_NAME_TYPE packetName)
    {
        this.Url = Url;
        this.PacketName = packetName.ToString();
    }
}

public class ReceivePacketBase
{
    public int ReturnCode;

    public ReceivePacketBase(int returnCode)
    {
        ReturnCode = returnCode;
    }
}
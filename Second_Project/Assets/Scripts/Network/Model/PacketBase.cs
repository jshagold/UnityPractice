public class SendPacketBase
{
    public string PacketName;

    public SendPacketBase(PACKET_NAME_TYPE packetName)
    {
        PacketName = packetName.ToString();
    }
}

public class ReceivePacketBase
{
    public readonly int ReturnCode;

    public ReceivePacketBase(int returnCode)
    {
        ReturnCode = returnCode;
    }
}
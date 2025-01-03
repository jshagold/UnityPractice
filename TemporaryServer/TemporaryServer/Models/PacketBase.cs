namespace TemporaryServer.Models
{
    public class ReceivePacketBase
    {
        public string PacketName;

        public ReceivePacketBase(string packetName)
        {
            PacketName = packetName;
        }
    }

    public class SendPacketBase
    {
        public string PacketName;
        public int ReturnCode;

        public SendPacketBase(PACKET_NAME_TYPE packetName, RETURN_CODE returnCode)
        {
            PacketName = packetName.ToString();
            ReturnCode = (int)returnCode;
        }
    }
}

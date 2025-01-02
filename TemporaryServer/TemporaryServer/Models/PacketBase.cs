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
        public readonly int ReturnCode;

        public SendPacketBase(string packetName, int returnCode)
        {
            ReturnCode = returnCode;
        }
    }
}

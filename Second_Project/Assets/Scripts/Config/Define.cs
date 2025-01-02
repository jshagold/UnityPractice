//public class PacketName
//{
//    public const string ApplicationConfig = "ApplicationConfig";
//}

public enum PACKET_NAME_TYPE
{
    ApplicationConfig,
}



public enum SCENE_TYPE
{
    Init,
    Lobby,
    Loading,
    InGame,
}

public enum ENVIRONMENT_TYPE
{
    Dev = 1,
    Stage = 2,
    Live = 3,
}

public enum OS_TYPE
{
    Android = 1,
    IOS = 2,
}
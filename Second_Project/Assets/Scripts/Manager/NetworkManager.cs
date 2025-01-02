using System;


public class CertHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 여기에서 서버의 인증서를 검증하는 로직을 작성할 수 있습니다.
        // 기본적으로는 true를 반환하여 모든 인증서를 허용합니다.
        return true;
    }
}

public class SendPacketBase
{
    public readonly string PacketName;

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

public class ApplicationConfigSendPacket : SendPacketBase
{
    // environment - dev, stage, live
    // os type - Android, IOS
    // version - 1.0.0

    public int E_ENVIRONMENT_TYPE;
    public int E_OS_TYPE;
    public string AppVersion;

    public ApplicationConfigSendPacket(PACKET_NAME_TYPE packetName, ENVIRONMENT_TYPE e_ENVIRONMENT_TYPE, OS_TYPE e_OS_TYPE, string appVersion) : base(packetName)
    {
        E_ENVIRONMENT_TYPE = (int)e_ENVIRONMENT_TYPE;
        E_OS_TYPE = (int)e_OS_TYPE;
        AppVersion = appVersion;
    }
}

public class ApplicationConfigReceivePacket : ReceivePacketBase
{
    public readonly string ApiUrl;

    public ApplicationConfigReceivePacket(int returnCode, string apiUrl) : base(returnCode)
    {
        ApiUrl = apiUrl;
    }
}

public class NetworkManager : ManagerBase
{
    private string apiUrl;
	
	private void Awake()
	{
		DontDestroy<NetworkManager>();
	}

	public void SetInit(string apiUrl)
	{
        this.apiUrl = apiUrl;
	}

    public void SendPacket()
    {
        StartCoroutine(C_SendPacket());
    }

    public IEnumerator C_SendPacket()
    {
        ApplicationConfigSendPacket applicationConfigSendPacket 
            = new ApplicationConfigSendPacket(
                PacketName.ApplicationConfig, 
                Config.E_ENVIRONMENT_TYPE, 
                ApplicationConfigSendPacket.E_OS_TYPE, 
                Config.APP_VERSION
                );

        string packet = JsonUtility.ToJson( applicationConfigSendPacket );
        Debug.Log("[NetworkManager Send Packet]" + packet);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(this.apiUrl, packet))
        {
            // HTTPS 통신을 위한 보안 설정
            request.certificateHandler = new CertHandler();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // 성공적으로 데이터를 가져왔을 때 처리
                string jsonData = request.downloadHandler.text;
                Debug.Log("Received Data: " + jsonData);

                // 여기서부터 JSON 데이터를 원하는 방식으로 처리하면 된다.
                // 예를 들어, JSON 데이터를 C#객체로 변환하려면 JsonUtility.FromJson<T>()를 사용한다.
                // 예: YourDataObject data = JsonUtility.FromJson<YourDataObject>(jsonData);
            }
        }

        byte[] bytes = new System.Text.UTF8Encoding().GetBytes(packet);
        request.uploadHandler = new UploadHandlerRaw(bytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");


        

    }


    //public IEnumerator C_SendPacket<T>(SendPacketBase sendPacketBase, Action<ReceivePacketBase> action = null) where T : ReceivePacketBase
    //{
    //    string packet = JsonUtility.ToJson(sendPacketBase);
    //    Debug.Log("[NetworkManager Send Packet] " + packet);

    //    try
    //    {
    //        packet = AesEncrypt.EncryptString(packet, Config.AES_KEY, out string iv);
    //        Debug.Log("Encrypt: " + packet);
    //        Debug.Log("IV: " + iv);
    //        packet = iv + "|" + packet;
    //    }
    //    catch (Exception e)
    //    {
    //        packet = string.Empty;
    //    }
    //    if (!string.IsNullOrEmpty(packet))
    //    {
    //        yield return null;
    //    }


    //    using (UnityWebRequest request = UnityWebRequest.PostWwwForm(sendPacketBase.Url, packet))
    //    {
    //        byte[] bytes = new System.Text.UTF8Encoding().GetBytes(packet);
    //        request.uploadHandler = new UploadHandlerRaw(bytes);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("Content-Type", "application/json");

    //        // HTTPS 통신을 위한 보안 설정
    //        request.certificateHandler = new CertHandler();

    //        this.initScene_UI.LoadingGear.EnableGear();

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            Debug.LogError("Error: " + request.error);
    //            yield return null;
    //            this.initScene_UI.LoadingGear.DisableGear();
    //            action?.Invoke(new ReceivePacketBase((int)RETURN_CODE.Error));
    //        }
    //        else
    //        {
    //            // 성공적으로 데이터를 가져왔을 때 처리
    //            string jsonData = request.downloadHandler.text;
    //            Debug.Log("Received Data: " + jsonData);

    //            T receivePacket = JsonUtility.FromJson<T>(jsonData);
    //            yield return receivePacket;
    //            this.initScene_UI.LoadingGear.DisableGear();
    //            action?.Invoke(receivePacket);
    //        }
    //    }
    //}

}

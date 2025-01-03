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

public class NetworkManager : ManagerBase
{
	private void Awake()
	{
		DontDestroy<NetworkManager>();
	}

	public void SetInit()
	{
	}

    //public void SendPacket(SendPacketBase sendPacketBase)
    //{
    //    StartCoroutine(C_SendPacket(sendPacketBase));
    //}

    public IEnumerator C_SendPacket<T>(SendPacketBase sendPacketBase) where T : ReceivePacketBase
    {
        string packet = JsonUtility.ToJson(sendPacketBase);
        Debug.Log("[NetworkManager Send Packet]" + packet);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(sendPacketBase.Url, packet))
        {
            byte[] bytes = new System.Text.UTF8Encoding().GetBytes(packet);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // HTTPS 통신을 위한 보안 설정
            request.certificateHandler = new CertHandler();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                yield return null;
            }
            else
            {
                // 성공적으로 데이터를 가져왔을 때 처리
                string jsonData = request.downloadHandler.text;
                Debug.Log("Received Data: " + jsonData);

                T receivePacket = JsonUtility.FromJson<T>(jsonData);
                yield return receivePacket;
            }
        }
    }

}

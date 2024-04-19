using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public bool isServer;
    [Header("NetworkManager")] public NetworkRoomManagerExt netManager;
    private void Start()
    {
        if (isServer)
        {
            //�Է������ķ�ʽ����
            netManager.StartServer();
        }
        else
        {
            //�Կͻ��˵ķ�ʽ����
            //networkManager.StartClient();
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ClickConnenct()
    {
        //NetworkManager.singleton.StartClient();
        netManager.StartClient();
    }
}

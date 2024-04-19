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
            //以服务器的方式启动
            netManager.StartServer();
        }
        else
        {
            //以客户端的方式启动
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

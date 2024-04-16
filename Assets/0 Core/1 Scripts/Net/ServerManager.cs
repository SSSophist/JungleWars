using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public bool isServer;
    [Header("NetworkManager")] public NetworkManager networkManager;
    private void Start()
    {
        if (isServer)
        {
            //以服务器的方式启动
            networkManager.StartServer();
        }
        else
        {
            //以客户端的方式启动
            networkManager.StartClient();
        }

        DontDestroyOnLoad(gameObject);

    }
}

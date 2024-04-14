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
        //��������������
        if (isServer)
        {
            networkManager.StartServer();
        }

        DontDestroyOnLoad(gameObject);
    }
}

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
            //�Է������ķ�ʽ����
            networkManager.StartServer();
        }
        else
        {
            //�Կͻ��˵ķ�ʽ����
            networkManager.StartClient();
        }

        DontDestroyOnLoad(gameObject);

    }
}

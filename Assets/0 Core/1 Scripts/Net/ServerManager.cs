using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public string networkAddress;
    public bool isServer;
    [Header("NetworkManager")] public NetworkRoomManagerExt netManager;
    [Header("连接服务器按钮")] public GameObject connectButton;
    private void Awake()
    {
        if(NetworkClient.active)
            Destroy(gameObject);
    }
    private void Start()
    {
        if (isServer)
        {
            StartServer();
        }
        else
        {

        }
        /*
        if (isServer)
        {
            //以服务器的方式启动
            netManager.StartServer();
        }
        else
        {
            //以客户端的方式启动
            //networkManager.StartClient();
            
        }*/
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if(NetworkClient.active)
        {
            connectButton.SetActive(false);
        }
        else
        {
            connectButton.SetActive(true);
        }
    }
    private void StartServer()
    {
        Debug.Log("Starting server...");
        //以服务器的方式启动
        netManager.StartServer();
    }
    public void ClickConnenct()
    {
        if (NetworkClient.active)
        {
            RoomManager.st.Show("已连接至服务器");
            return;
        }
        RoomManager.st.Show("正在连接服务器...");
        netManager.networkAddress = networkAddress;
        netManager.StartClient();
    }
    private bool IsServer()
    {
#if UNITY_SERVER
        return true;
#else
        return false;
#endif
    }
    private void OnApplicationQuit()
    {
        // 确保在应用程序关闭时关闭客户端连接
        netManager.StopClient();
    }
    [Button("连接本地服务器")]
    public void ConnectLocalHost()
    {
        netManager.networkAddress = "localhost";
        netManager.StartClient();
    }
}
//NetworkManager.singleton.StartClient();
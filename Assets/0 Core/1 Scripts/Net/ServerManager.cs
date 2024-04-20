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
    [Header("���ӷ�������ť")] public GameObject connectButton;
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
            //�Է������ķ�ʽ����
            netManager.StartServer();
        }
        else
        {
            //�Կͻ��˵ķ�ʽ����
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
        //�Է������ķ�ʽ����
        netManager.StartServer();
    }
    public void ClickConnenct()
    {
        if (NetworkClient.active)
        {
            RoomManager.st.Show("��������������");
            return;
        }
        RoomManager.st.Show("�������ӷ�����...");
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
        // ȷ����Ӧ�ó���ر�ʱ�رտͻ�������
        netManager.StopClient();
    }
    [Button("���ӱ��ط�����")]
    public void ConnectLocalHost()
    {
        netManager.networkAddress = "localhost";
        netManager.StartClient();
    }
}
//NetworkManager.singleton.StartClient();
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MenuManager : MonoBehaviour
{
    public NetworkManager manager;
    
    public List<RoomItemInfo> roomItems;
    public RoomItemInfo curRoomItem;

    //创建房间
    public void OnCreateRoom()
    {
        manager.StartHost();
    }
    //加入服务器
    public void OnConnectRoom()
    {
        manager.StartClient();
    }

    public void OnStopButton()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            manager.StopHost(); // stop host if host mode
            manager.StopClient();  // stop client if host mode, leaving server up
        }
        else if (NetworkClient.isConnected)
        {
            manager.StopClient(); // stop client if client-only
        }
        else if (NetworkServer.active)
        {
            manager.StopServer();// stop server if server-only
        }
    }
    public void OnStopClient()
    {
        manager.StopClient();
    }
}

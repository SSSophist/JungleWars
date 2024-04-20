using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public NetworkRoomManagerExt net=> NetworkRoomManagerExt.singleton;
    public NetworkRoomPlayer curNetRoomPlayer;
    public int index;
    public bool isReady;

    public GameObject roomPanel;
    public bool canShowRoomPlayers;
    [ShowIf("canShowRoomPlayers")]public List<NetworkRoomPlayer> roomPlayers => net.roomSlots;
    public string GameplayScene = "1 Main";
    public Button startGameButton;
    public Button readyButton;
    [FoldoutGroup("Set")] public GameObject esePanel;
    [FoldoutGroup("Set")] public TMP_Text readyText;
    [FoldoutGroup("Set")] public GameObject loginPanel;
    [FoldoutGroup("Set")][LabelText("提示文字")] public TMP_Text tipText;
   public static RoomManager st;

    [FoldoutGroup("Set")][LabelText("房间预制体")] public RoomPlayerItem roomPlayerItemPrefab;
    [FoldoutGroup("Set")][LabelText("房间父物体")] public Transform itemUIContainer;
    [Sirenix.OdinInspector.ShowInInspector][Header("房间Item")] List<RoomPlayerItem> items = new();
    private void Awake()
    {
        st = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            esePanel.SetActive(!esePanel.activeInHierarchy);
        }
    }

    // 点击准备
    public void OnReady()
    {
        isReady = !isReady;
        curNetRoomPlayer.CmdChangeReadyState(isReady);
        readyText.text = isReady ? "取消准备" : "准备";
    }
    // 点击创建房间
    public void OnCreateRoom()
    {
        NetworkManager.singleton.StartHost();
        //显示房间内玩家列表
    }
    // 点击加入房间
    public void OnJoinRoom()
    {
        NetworkManager.singleton.StartClient();
    }

    public void UpdateRoomInfo()
    {
        //Debug.Log("更新房间信息" + roomPlayers.Count);
        items.Clear();
        foreach (NetworkRoomPlayer room in roomPlayers)
        {
            RoomPlayerItem item = room.GetComponent<RoomPlayerItem>();
            item.transform.SetParent(itemUIContainer);
            item.UpdateInfo(room.index, "Player " + room.index, room.readyToBegin);
            items.Add(item.GetComponent<RoomPlayerItem>());
        }
    }
    // 返回房间
    public void ReturnRoom()
    {
        roomPanel.SetActive(true);
        net.ReturnRoom();
    }
    // 展示房间
    public void ShowRoom()
    {
        roomPanel.SetActive(true);
    }


    public void UpdateReadyState(bool isReady)
    {
        this.isReady = isReady;
        items[index].UpdateReadyInfo(isReady);
    }
    public void ShowStartGameButton()
    {
        startGameButton.gameObject.SetActive(true);
        //if(curNetRoomPlayer.index == 0)
    }
    public void HideStartGameButton()
    {
        startGameButton.gameObject.SetActive(false);
    }
    public void OnStartGameButton()
    {
        curNetRoomPlayer.CmdOnStartGame();
        startGameButton.gameObject.SetActive(false);
    }
    // 当开始游戏时执行的东西
    public void OnStartGame()
    {
        roomPanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    void ShowTip()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            // host mode
            //tipText.text = $"<b>Host</b>: running via {Transport.active}";
            tipText.text = $"房间已经创建";
        }
        else if (NetworkServer.active)
        {
            // server only
            tipText.text = $"<b>Server</b>: running via {Transport.active}";
        }
        else if (NetworkClient.isConnected)
        {
            // client only
            tipText.text = $"<b>Client</b>: connected to {net.networkAddress} via {Transport.active}";
        }
    }
    public void Show(string content)
    {
        tipText.text = content;
    }

    public void ClickExitButton()
    {
        Application.Quit();
    }
}
public struct RoomData
{
    public int index;
    //public bool allPlayerReady;
}
public class RoomPlayerInfo
{
    public int index;
    public bool isReady;
    //public bool allPlayerReady;
}
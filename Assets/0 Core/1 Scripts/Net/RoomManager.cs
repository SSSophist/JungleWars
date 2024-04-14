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
    public TMP_Text readyText;
    [FoldoutGroup("Set")][LabelText("��ʾ����")] public TMP_Text tipText;
   public static RoomManager st;

    [FoldoutGroup("Set")][LabelText("����Ԥ����")] public RoomPlayerItem roomPlayerItemPrefab;
    [FoldoutGroup("Set")][LabelText("���丸����")] public Transform itemUIContainer;
    [Sirenix.OdinInspector.ShowInInspector][Header("����Item")] List<RoomPlayerItem> items = new();
    private void Awake()
    {
        st = this;
    }



    public void OnReady()
    {
        isReady = !isReady;
        roomPlayers[index].CmdChangeReadyState(isReady);
        readyText.text = isReady ? "ȡ��׼��" : "׼��";
    }

    public void OnCreateRoom()
    {
        NetworkManager.singleton.StartHost();
        //��ʾ����������б�
    }
    public void OnJoinRoom()
    {
        NetworkManager.singleton.StartClient();
    }

    public void UpdateRoomInfo()
    {
        //Debug.Log("���·�����Ϣ" + roomPlayers.Count);
        items.Clear();
        foreach (NetworkRoomPlayer room in roomPlayers)
        {
            RoomPlayerItem item = room.GetComponent<RoomPlayerItem>();
            item.transform.SetParent(itemUIContainer);
            item.UpdateInfo(room.index, "Player " + room.index, room.readyToBegin);
            items.Add(item.GetComponent<RoomPlayerItem>());
        }
    }
    // ���ط���
    public void ReturnRoom()
    {
        roomPanel.SetActive(true);
        net.ReturnRoom();
    }
    // չʾ����
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
    }
    public void OnStartGameButton()
    {
        if (net.allPlayersReady)
        {
            // set to false to hide it in the game scene
            curNetRoomPlayer.CmdOnStartGame();
            startGameButton.gameObject.SetActive(false);
            net.ServerChangeScene(GameplayScene);
        }
    }
    // ����ʼ��Ϸʱ
    public void OnStartGame()
    {
        roomPanel.SetActive(false);
    }

    void ShowTip()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            // host mode
            //tipText.text = $"<b>Host</b>: running via {Transport.active}";
            tipText.text = $"�����Ѿ�����";
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
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
    [FoldoutGroup("Set")][LabelText("��ʾ����")] public TMP_Text tipText;
   public static RoomManager st;

    [FoldoutGroup("Set")][LabelText("����Ԥ����")] public RoomPlayerItem roomPlayerItemPrefab;
    [FoldoutGroup("Set")][LabelText("���丸����")] public Transform itemUIContainer;
    [Sirenix.OdinInspector.ShowInInspector][Header("����Item")] List<RoomPlayerItem> items = new();
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

    // ���׼��
    public void OnReady()
    {
        isReady = !isReady;
        curNetRoomPlayer.CmdChangeReadyState(isReady);
        readyText.text = isReady ? "ȡ��׼��" : "׼��";
    }
    // �����������
    public void OnCreateRoom()
    {
        NetworkManager.singleton.StartHost();
        //��ʾ����������б�
    }
    // ������뷿��
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
    // ����ʼ��Ϸʱִ�еĶ���
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
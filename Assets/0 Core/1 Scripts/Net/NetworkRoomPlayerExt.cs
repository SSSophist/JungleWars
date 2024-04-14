using Mirror;
using Org.BouncyCastle.Asn1.Mozilla;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    public override void OnDisable()
    {

    }
    /// <summary>Like Start(), but only called on client and host.</summary>
    public override void OnStartClient()
    {
        if(isLocalPlayer)
            RoomManager.st.curNetRoomPlayer = this;
        //transform.SetParent(RoomManager.st.itemUIContainer);
        //RoomManager.st.UpdateRoom();
        Debug.Log($"OnStartClient {gameObject}" + index);
    }

    //���ͻ��˽��뷿��
    public override void OnClientEnterRoom()
    {
        //���·�����Ϣ
        RoomManager.st.UpdateRoomInfo();
        Debug.Log($"OnClientEnterRoom {index}" + RoomManager.st.roomPlayers.Count);
        //Debug.Log($"NetworkRoomPlayerExt OnClientEnterRoom {SceneManager.GetActiveScene().path}");
    }

    //���ͻ����뿪����
    public override void OnClientExitRoom()
    {
        Debug.Log($"OnClientExitRoom {index}");
        RoomManager.st.OnStartGame();
        //Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
    }

    public override void IndexChanged(int oldIndex, int newIndex)
    {
        //Debug.Log($"IndexChanged {newIndex}");
        if(isLocalPlayer)
        {
            Debug.Log("������������޸�index");
            RoomManager.st.index = newIndex;
        }
        else
        {
            Debug.Log("��һ����������޸�index");
        }
    }
    //׼��״̬�ı�
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (isLocalPlayer)
        {
            Debug.Log("������������޸�ReadyState");
            //RoomManager.st.isReady = newReadyState;
            GetComponent<RoomPlayerItem>().UpdateReadyInfo(newReadyState);
            //RoomManager.st.UpdateReadyState(newReadyState);
        }
        else
        {
            Debug.Log("��һ����������޸�ReadyState");
            GetComponent<RoomPlayerItem>().UpdateReadyInfo(newReadyState);
        }
    }

    public void Win()
    {

    }
}
using Mirror;

using UnityEngine;


[AddComponentMenu("")]
public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    public override void OnDisable()
    {

    }
    public override void OnStartClient()
    {
        if(isLocalPlayer)
            RoomManager.st.curNetRoomPlayer = this;
        //transform.SetParent(RoomManager.st.itemUIContainer);
        //RoomManager.st.UpdateRoom();
        Debug.Log($"OnStartClient {gameObject}" + index);
    }

    //当客户端进入房间
    public override void OnClientEnterRoom()
    {
        //更新房间信息
        RoomManager.st.UpdateRoomInfo();
        Debug.Log($"OnClientEnterRoom 当客户端进入房间{index}" + RoomManager.st.roomPlayers.Count);
        //Debug.Log($"NetworkRoomPlayerExt OnClientEnterRoom {SceneManager.GetActiveScene().path}");
    }

    //当客户端离开房间
    public override void OnClientExitRoom()
    {
        Debug.Log($"客户端离开房间 OnClientExitRoom {index}");
        //RoomManager.st.OnStartGame();
        //Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
    }

    public override void IndexChanged(int oldIndex, int newIndex)
    {
        //Debug.Log($"IndexChanged {newIndex}");
        if(isLocalPlayer)
        {
            Debug.Log("本地玩家正在修改index");
            RoomManager.st.index = newIndex;
        }
        else
        {
            Debug.Log("另一个玩家正在修改index");
        }
    }
    //准备状态改变
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (isLocalPlayer)
        {
            Debug.Log("本地玩家正在修改ReadyState");
            //RoomManager.st.isReady = newReadyState;
            GetComponent<RoomPlayerItem>().UpdateReadyInfo(newReadyState);
            //RoomManager.st.UpdateReadyState(newReadyState);
        }
        else
        {
            Debug.Log("另一个玩家正在修改ReadyState");
            GetComponent<RoomPlayerItem>().UpdateReadyInfo(newReadyState);
        }
    }

    public void Win()
    {

    }
}
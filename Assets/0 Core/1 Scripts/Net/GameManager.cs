using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager st;
    public int playerIndex;

    private void Awake()
    {
        st = this;
    }

    // 在所有客户端上同步执行的 RPC
    // 胜利信息的Rpc消息
    [ClientRpc]
    public void RpcDisplayVictoryPanel(int playerIndex)
    {
        //isLocalPlayer && 
        if (playerIndex == this.playerIndex)
        {
            Debug.Log(netId + "赢");
            // 显示胜利面板
            ItemManager.st.Win();
        }
        else
        {
            Debug.Log(netId + "输");
            // 显示失败面板
            ItemManager.st.Lose();
        }
    }


    public void PlayerWins(int playerIndex)
    {
        Debug.Log(isOwned.ToString() + isServer.ToString() + isClient.ToString() + isLocalPlayer.ToString());
        // 在胜利发生时调用Rpc消息
        CmdPlayerWins(playerIndex);

    }

    // 当胜利条件达成时调用此方法
    public void CmdPlayerWins(int playerIndex)
    {
       
        // 在胜利发生时调用Rpc消息
        RpcDisplayVictoryPanel(playerIndex);
       
    }
}

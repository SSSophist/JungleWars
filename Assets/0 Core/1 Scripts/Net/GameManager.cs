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

    // 胜利信息的Rpc消息
    [ClientRpc]
    void RpcDisplayVictoryPanel(uint targetNetID)
    {
        //isLocalPlayer && 
        if (targetNetID == netId)
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

    // 当胜利条件达成时调用此方法
    public void PlayerWins()
    {
        // 在胜利发生时调用Rpc消息
        RpcDisplayVictoryPanel(netId);
    }
}

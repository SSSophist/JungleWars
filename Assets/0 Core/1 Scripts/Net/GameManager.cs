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

    // �����пͻ�����ͬ��ִ�е� RPC
    // ʤ����Ϣ��Rpc��Ϣ
    [ClientRpc]
    public void RpcDisplayVictoryPanel(int playerIndex)
    {
        //isLocalPlayer && 
        if (playerIndex == this.playerIndex)
        {
            Debug.Log(netId + "Ӯ");
            // ��ʾʤ�����
            ItemManager.st.Win();
        }
        else
        {
            Debug.Log(netId + "��");
            // ��ʾʧ�����
            ItemManager.st.Lose();
        }
    }


    public void PlayerWins(int playerIndex)
    {
        Debug.Log(isOwned.ToString() + isServer.ToString() + isClient.ToString() + isLocalPlayer.ToString());
        // ��ʤ������ʱ����Rpc��Ϣ
        CmdPlayerWins(playerIndex);

    }

    // ��ʤ���������ʱ���ô˷���
    public void CmdPlayerWins(int playerIndex)
    {
       
        // ��ʤ������ʱ����Rpc��Ϣ
        RpcDisplayVictoryPanel(playerIndex);
       
    }
}

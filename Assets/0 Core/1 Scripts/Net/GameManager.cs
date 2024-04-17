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

    // ʤ����Ϣ��Rpc��Ϣ
    [ClientRpc]
    void RpcDisplayVictoryPanel(uint targetNetID)
    {
        //isLocalPlayer && 
        if (targetNetID == netId)
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

    // ��ʤ���������ʱ���ô˷���
    public void PlayerWins()
    {
        // ��ʤ������ʱ����Rpc��Ϣ
        RpcDisplayVictoryPanel(netId);
    }
}

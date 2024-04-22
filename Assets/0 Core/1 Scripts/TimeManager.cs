using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : NetworkBehaviour
{
    [SyncVar] public float curTime;
    public TMP_Text timeText,winTimeText,loseTimeText;

    float startTime;
    public bool canTime = true;
    public static TimeManager st;
    public override void OnStartServer()
    {
        st = this;
        Debug.Log("Reset Time");
        startTime = Time.time;
    }

    private void Update()
    {
        if (!isServer)
            return;

        // �����Ϸ��ʼ�����µ�ǰʱ��
        if (canTime)
        {
            curTime = Time.time - startTime;

            // �ڷ������ϸ���ʱ�䣬Ȼ��ʱ��ͬ�������пͻ���
            RpcUpdateTime(curTime);
        }
    }
    // �ɷ�������������ʱ����µ����пͻ���
    [ClientRpc]
    private void RpcUpdateTime(float time)
    {
        // �����пͻ������ֶ�����ʱ���ı�
        timeText.text = "Time: " + time.ToString("F2");
        winTimeText.text = "��ʱ��" + time.ToString("F2") + "��";
        loseTimeText.text = "��ʱ��" + time.ToString("F2") + "��";
    }
    //ֹͣʱ��
    public void StopTime()
    {
        Debug.Log(isOwned.ToString() + isServer.ToString() + isClient.ToString() + isLocalPlayer.ToString());
        canTime = false;
    }

    //ֹͣʱ��
    public void StartTime()
    {
        canTime = true;
    }
}
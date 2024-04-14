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
    public void Start()
    {
        st = this;
        startTime = Time.time;
    }

    private void Update()
    {
        if (!isServer)
            return;

        // 如果游戏开始，更新当前时间
        if (canTime)
        {
            curTime = Time.time - startTime;

            // 在服务器上更新时间，然后将时间同步到所有客户端
            RpcUpdateTime(curTime);
        }
    }
    // 由服务器调用来将时间更新到所有客户端
    [ClientRpc]
    private void RpcUpdateTime(float time)
    {
        // 在所有客户端上手动更新时间文本
        timeText.text = "Time: " + time.ToString("F2");
        winTimeText.text = "用时：" + time.ToString("F2") + "秒";
        loseTimeText.text = "用时：" + time.ToString("F2") + "秒";
    }

    //停止时间
    [Command]
    public void CmdStopTime()
    {
        canTime = false;
        RpcStopTime();
    }
    [ClientRpc]
    public void RpcStopTime()
    {
        canTime = false;
    }
    //停止时间
    public void StartTime()
    {
        canTime = true;
    }
}
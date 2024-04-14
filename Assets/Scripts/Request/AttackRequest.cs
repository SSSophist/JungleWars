using System;
using System.Collections.Generic;
using Common;
using UnityEngine;


public class AttackRequest:BaseRequest
{
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Attack ;
        base.Awake();
    }
    public void SendRequest(int damage)
    {
        base.SendRequest(damage.ToString());
    }
}


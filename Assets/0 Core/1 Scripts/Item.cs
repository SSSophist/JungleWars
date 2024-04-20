using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using TinyGiantStudio.Text;
using Sirenix.OdinInspector;
using Mirror;

public class Item : NetworkBehaviour
{
    [SyncVar] public ItemInfo itemInfo;
    public TweenSettings<Quaternion> ts;
    public Modular3DText text3D;

    public override void OnStartClient()
    {
        text3D.Text = itemInfo.num.ToString();
        text3D.UpdateText(itemInfo.num.ToString());
    }
    public void Init(ItemInfo itemInfo)
    {
        this.itemInfo = itemInfo;

        text3D.Text = itemInfo.num.ToString();
        text3D.UpdateText(itemInfo.num.ToString());
    }

    [ClientRpc]
    public void RpcInit(ItemInfo itemInfo)
    {
        this.itemInfo = itemInfo;

        text3D.Text = itemInfo.num.ToString();
        text3D.UpdateText(itemInfo.num.ToString());
    }
    private void Start()
    {
        Tween.Rotation(transform, ts);
    }
    
    private void OnDestroy()
    {
        if(isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}

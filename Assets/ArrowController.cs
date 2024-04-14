using Common;
using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ArrowController : NetworkBehaviour
{
    [SyncVar] public PInfo pInfo;
    [Header("»÷ÖÐÌØÐ§")]public GameObject explosionEffect;
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), 4f);
    }

    public void Init(PInfo pInfo)
    {
        this.pInfo = pInfo;
    }
    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    // ServerCallback because we don't want a warning
    // if OnTriggerEnter is called on the client
    [ServerCallback]//Callback [Command]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")&& isServer)
        {
            GameObject vfx = Instantiate(explosionEffect,transform.position,Quaternion.identity);
            NetworkServer.Spawn(vfx);

            if(pInfo.pc.items.Count <4)
            {
               // pInfo.pc.pickedNum = other.GetComponent<Item>().itemInfo.num;
                pInfo.pc.RpcUpdateNum(pInfo, other.GetComponent<Item>().itemInfo.num);
            }
            DestroySelf();
            DestroyOther(other);
        }
    }
    [Server]
    void DestroyOther(Collider other)
    {
        NetworkServer.Destroy(other.gameObject);
    }
}

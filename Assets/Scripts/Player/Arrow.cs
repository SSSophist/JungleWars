using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Mirror;
public class Arrow : MonoBehaviour
{
    public int speed = 5;
    public GameObject explosionEffect;
    private Rigidbody rb;
    public bool isLocal;
    public RoleType roleType;
    void Start () {
        Destroy(gameObject, 4f);
	}

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_ShootPerson);
            if (isLocal)
            {
                bool playerIsLocal = other.GetComponent<PlayerInfo>().isLocal;
                if (isLocal != playerIsLocal)
                {
                    GameFacade.Instance.SendAttack( Random.Range(10,20) );
                }
            }
        }
        else
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_Miss);
        }
        GameObject.Instantiate(explosionEffect, transform.position, transform.rotation);
        GameObject.Destroy(this.gameObject);
    }
}

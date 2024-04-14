using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [System.Serializable]
    public struct ItemInfoNW
    {
        public Vector3 pos;
        public ItemInfo itemInfo;
    }

    public Vector2Int numRange = new(3,23); 
    [SyncVar] public float minInterval, maxInterval;
    [SyncVar] public int maxCount=50;
    [SyncVar] public int curCount;
    [SyncVar] public List<ItemInfoNW> hasSpawnItemInfos = new List<ItemInfoNW>();
    [SyncVar] public List<GameObject> hasSpawnItems = new List<GameObject>();
    public GameObject itemPrefab;
    public Vector3 randomAreaSize; 

    public override void OnStartServer()
    {
        Debug.Log("��ʼ����");
        StartCoroutine(StartSpawner());
    }
    /*
    public override void OnStartClient()
    {
        hasSpawnItems.RemoveAll(item => item == null);
         foreach (GameObject item in hasSpawnItems)
        {
            NetworkServer.Spawn(item);
        }
    }*/

    IEnumerator StartSpawner()
    {
        yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        // ������5��
        while (curCount <= 5)
        {
            curCount++;
            Spawn();
            yield return new WaitForSeconds(0.2f);
        }

        // ������5��
        while (true)
        {
            if(curCount<=maxCount)
            {
                //Debug.Log("����");
                curCount++;
                Spawn();

            }
                
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
        
    }

    public void Spawn()
    {
        ItemInfoNW itemInfoNW = new ItemInfoNW();
        itemInfoNW.pos = GetRandomPos();
        itemInfoNW.itemInfo.num = Random.Range(numRange.x, numRange.y);


        GameObject itemGO = Instantiate(itemPrefab, itemInfoNW.pos, Quaternion.identity);
        NetworkServer.Spawn(itemGO);
        //������Spaw��ִ��
        itemGO.GetComponent<Item>().RpcInit(itemInfoNW.itemInfo);

        hasSpawnItems.Add(itemGO);
        hasSpawnItemInfos.Add(itemInfoNW);
    }
    public Vector3 GetRandomPos()
    {
        // ��ȡ�������������λ��
        Vector3 center = transform.position;
        // ���㷽������İ�ߴ�
        float halfX = randomAreaSize.x / 2;
        float halfZ = randomAreaSize.z / 2;
        // �������λ��
        float randomX = Random.Range(center.x - halfX, center.x + halfX);
        float randomZ = Random.Range(center.z - halfZ, center.z + halfZ);
        float randomY = center.y; // Yλ����ͬ

        return new Vector3(randomX, randomY, randomZ);
    }
    // ������Unity�༭���и��ķ�������ĳߴ�
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, randomAreaSize);
    }
}

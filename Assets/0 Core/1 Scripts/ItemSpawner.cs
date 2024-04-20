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
    [SyncVar] public int maxCount = 50;
    //public SyncList<ItemInfoNW> hasSpawnItemInfos = new();
    [Mirror.ReadOnly] public SyncList<GameObject> hasSpawnItems = new();

    public GameObject itemPrefab;
    public Vector3 randomAreaSize; 

    public override void OnStartServer()
    {
        StartCoroutine(StartSpawner());
    }

    IEnumerator StartSpawner()
    {
        yield return new WaitForSeconds(1f);
        // 先生成5个
        while (hasSpawnItems.Count <= 5)
        {
            Spawn();
            yield return new WaitForSeconds(0.2f);
        }

        // 
        while (true)
        {
            if(hasSpawnItems.Count <= maxCount)
            {
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
        itemGO.GetComponent<Item>().Init(itemInfoNW.itemInfo);
        NetworkServer.Spawn(itemGO);

        //必须在Spaw后执行
        // 初始化信息
        //itemGO.GetComponent<Item>().RpcInit(itemInfoNW.itemInfo);

        hasSpawnItems.RemoveAll(item => item == null);
        hasSpawnItems.Add(itemGO);
        //hasSpawnItemInfos.Add(itemInfoNW);
    }
    public Vector3 GetRandomPos()
    {
        // 获取方形区域的中心位置
        Vector3 center = transform.position;
        // 计算方形区域的半尺寸
        float halfX = randomAreaSize.x / 2;
        float halfZ = randomAreaSize.z / 2;
        // 生成随机位置
        float randomX = Random.Range(center.x - halfX, center.x + halfX);
        float randomZ = Random.Range(center.z - halfZ, center.z + halfZ);
        float randomY = center.y; // Y位置相同

        return new Vector3(randomX, randomY, randomZ);
    }
    // 可以在Unity编辑器中更改方形区域的尺寸
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, randomAreaSize);
    }
}

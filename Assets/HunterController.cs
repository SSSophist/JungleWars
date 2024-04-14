using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Mirror;
using System;

[Serializable]
public class PInfo
{
    [SyncVar] public int index;
    public HunterController pc;
}
[Serializable]
public struct ItemInfo
{
    [SyncVar] public int num;

    public ItemInfo(int num)
    {
        this.num = num;
    }
}
public class HunterController : NetworkBehaviour
{
    public bool canControl = true;
    //仓库
    [SyncVar] public PInfo pInfo=new();
    [SyncVar] public List<ItemInfo> items = new();//[SyncVar(hook = nameof(OnPlayerInfoChanged))] 
    
    //[SyncVar(hook = nameof(OnItemChanged))] public int pickedNum;
    //玩家动画机
    public Animator animator;
    //移动速度
    [SyncVar] public float moveSpeed = 3f;
    //转向平滑度
    public float rotateSmooth = 10f;
    //玩家模型变换
    public Transform model;
    //相机变换
    public Transform cameraTF;

    public Transform shootingPoint; // 箭的发射点
    public GameObject arrowPrefab; // 箭的预制体
    public float arrowSpeed = 5f;
    public LayerMask groundLayer; // 地面的层级

    [SyncVar][Header("正在攻击")] public bool isAttack;

    [ClientRpc]
    public void RpcUpdateNum(PInfo pInfo, int num)
    {
        if(this.pInfo.index == pInfo.index)
        {
            items.Add(new ItemInfo(num));
            Debug.Log("物品更新" + pInfo.index);

            //更新本地玩家的UI
            if (isLocalPlayer)
            {
                Debug.Log("更新本地玩家的UI");
                ItemManager.st.UpdateUI(items);
            }
        }
    }
    [ClientRpc]
    public void RpcUpdateUI()
    {
        if(isLocalPlayer)
        {
            ItemManager.st.UpdateUI(items);
        }
    }
    //当客户端创建一个网络对象时，OnStartClient 在每个客户端上的该网络对象上被调用
    public override void OnStartClient()
    {
        // 获取玩家连接序号
        pInfo.pc = this;

        if(isLocalPlayer)
        {
            ItemManager.st.pc = this;
            cameraTF = Camera.main.transform;
            Camera.main.GetComponent<CameraController>().SetTarget(transform);
        }
    }
    // 这个方法在玩家生成时被调用
    public override void OnStartServer()
    {
        base.OnStartServer();
      
        pInfo.index = GetPlayerIndex();

    }

    // 获取玩家连接序号的方法
    private int GetPlayerIndex()
    {
        // 获取所有连接的玩家
        Dictionary<int, NetworkConnectionToClient> connections = NetworkServer.connections;

        // 遍历连接，查找自己的连接
        int i = 1;
        foreach (KeyValuePair<int, NetworkConnectionToClient> connection in connections)
        {
            if (connection.Value == connectionToClient)
            {
                // 玩家连接序号从1开始，所以返回i
                return i;
            }
            i++;
        }

        // 如果没有找到，返回0或者其他适当的值
        return 0;
    }
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (!canControl)
            return;

        // 获取玩家输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // 控制动画状态和移动
        if (moveDirection.magnitude >= 0.1f)
        {
            //根据Camera方向
            moveDirection = cameraTF.TransformDirection(moveDirection);
            MovePlayer(moveDirection);  //移动玩家
            RotatePlayer(moveDirection);//旋转玩家
            animator.SetBool(name: "IsWalk", value: true);
        }
        else
        {
            animator.SetBool(name: "IsWalk", value: false);
        }

        if (Input.GetButtonDown("Fire1")) // 当按下鼠标左键
        {
            Shoot();
        }
    }
    void MovePlayer(Vector3 moveDirection)
    {
        // 将垂直方向设为0，只在水平方向移动
        moveDirection.y = 0f;
        // 计算移动向量并移动玩家
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
    void RotatePlayer(Vector3 moveDirection)
    {
        if (isAttack)
            return;

        // 通过移动方向旋转玩家
        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        // 将旋转限制为只影响 Y 轴
        toRotation = Quaternion.Euler(0f, toRotation.eulerAngles.y, 0f);
        model.rotation = Quaternion.Slerp(model.rotation, toRotation, Time.deltaTime * rotateSmooth);
    }

    Tween attackTween;
    public Sequence shootSeq;
    [Header("射击间隔")] public float shootCDTime = 0.2f;
    //朝某方向射击

    public void Shoot()
    {
        if (isAttack)
        {
            return;
        }
        isAttack = true;

        //shootSeq
        attackTween.Stop();
        attackTween = Tween.Delay(shootCDTime, () => isAttack = false);


        // 从相机位置发射一条射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 如果射线碰撞到了地面，则发射箭
            if (hit.collider.CompareTag("Ground"))
            {
                // 计算射击方向
                Vector3 targetPosition = hit.point;

                targetPosition.y = shootingPoint.position.y; // 保持在xz平面上
                Vector3 shootDirection = (targetPosition - shootingPoint.position).normalized;
                CmdShoot(shootDirection);
                model.forward = shootDirection;
            }
        }
    }
    [Command]
    public void CmdShoot(Vector3 shootDirection)//Vector3 dir
    {
        //Debug.Log("CmdShoot");
        // 实例化箭并发射
        GameObject arrowGO = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.identity);
        ArrowController arrow = arrowGO.GetComponent<ArrowController>();
        arrow.Init(pInfo);
        arrowGO.transform.forward = shootDirection;
        //arrowGO.St
        Rigidbody arrowRigidbody = arrowGO.GetComponent<Rigidbody>();
        arrowRigidbody.velocity = shootDirection * arrowSpeed;

        //网络同步
        NetworkServer.Spawn(arrowGO);
        
        RpcOnFire();
    }
    // this is called on the tank that fired for all observers
    [ClientRpc]
    void RpcOnFire()
    {
        animator.Play("Shoot");
    }
}

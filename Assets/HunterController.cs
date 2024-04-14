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
    //�ֿ�
    [SyncVar] public PInfo pInfo=new();
    [SyncVar] public List<ItemInfo> items = new();//[SyncVar(hook = nameof(OnPlayerInfoChanged))] 
    
    //[SyncVar(hook = nameof(OnItemChanged))] public int pickedNum;
    //��Ҷ�����
    public Animator animator;
    //�ƶ��ٶ�
    [SyncVar] public float moveSpeed = 3f;
    //ת��ƽ����
    public float rotateSmooth = 10f;
    //���ģ�ͱ任
    public Transform model;
    //����任
    public Transform cameraTF;

    public Transform shootingPoint; // ���ķ����
    public GameObject arrowPrefab; // ����Ԥ����
    public float arrowSpeed = 5f;
    public LayerMask groundLayer; // ����Ĳ㼶

    [SyncVar][Header("���ڹ���")] public bool isAttack;

    [ClientRpc]
    public void RpcUpdateNum(PInfo pInfo, int num)
    {
        if(this.pInfo.index == pInfo.index)
        {
            items.Add(new ItemInfo(num));
            Debug.Log("��Ʒ����" + pInfo.index);

            //���±�����ҵ�UI
            if (isLocalPlayer)
            {
                Debug.Log("���±�����ҵ�UI");
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
    //���ͻ��˴���һ���������ʱ��OnStartClient ��ÿ���ͻ����ϵĸ���������ϱ�����
    public override void OnStartClient()
    {
        // ��ȡ����������
        pInfo.pc = this;

        if(isLocalPlayer)
        {
            ItemManager.st.pc = this;
            cameraTF = Camera.main.transform;
            Camera.main.GetComponent<CameraController>().SetTarget(transform);
        }
    }
    // ����������������ʱ������
    public override void OnStartServer()
    {
        base.OnStartServer();
      
        pInfo.index = GetPlayerIndex();

    }

    // ��ȡ���������ŵķ���
    private int GetPlayerIndex()
    {
        // ��ȡ�������ӵ����
        Dictionary<int, NetworkConnectionToClient> connections = NetworkServer.connections;

        // �������ӣ������Լ�������
        int i = 1;
        foreach (KeyValuePair<int, NetworkConnectionToClient> connection in connections)
        {
            if (connection.Value == connectionToClient)
            {
                // ���������Ŵ�1��ʼ�����Է���i
                return i;
            }
            i++;
        }

        // ���û���ҵ�������0���������ʵ���ֵ
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

        // ��ȡ�������
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ƶ�����
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // ���ƶ���״̬���ƶ�
        if (moveDirection.magnitude >= 0.1f)
        {
            //����Camera����
            moveDirection = cameraTF.TransformDirection(moveDirection);
            MovePlayer(moveDirection);  //�ƶ����
            RotatePlayer(moveDirection);//��ת���
            animator.SetBool(name: "IsWalk", value: true);
        }
        else
        {
            animator.SetBool(name: "IsWalk", value: false);
        }

        if (Input.GetButtonDown("Fire1")) // ������������
        {
            Shoot();
        }
    }
    void MovePlayer(Vector3 moveDirection)
    {
        // ����ֱ������Ϊ0��ֻ��ˮƽ�����ƶ�
        moveDirection.y = 0f;
        // �����ƶ��������ƶ����
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
    void RotatePlayer(Vector3 moveDirection)
    {
        if (isAttack)
            return;

        // ͨ���ƶ�������ת���
        Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        // ����ת����ΪֻӰ�� Y ��
        toRotation = Quaternion.Euler(0f, toRotation.eulerAngles.y, 0f);
        model.rotation = Quaternion.Slerp(model.rotation, toRotation, Time.deltaTime * rotateSmooth);
    }

    Tween attackTween;
    public Sequence shootSeq;
    [Header("������")] public float shootCDTime = 0.2f;
    //��ĳ�������

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


        // �����λ�÷���һ������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // ���������ײ���˵��棬�����
            if (hit.collider.CompareTag("Ground"))
            {
                // �����������
                Vector3 targetPosition = hit.point;

                targetPosition.y = shootingPoint.position.y; // ������xzƽ����
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
        // ʵ������������
        GameObject arrowGO = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.identity);
        ArrowController arrow = arrowGO.GetComponent<ArrowController>();
        arrow.Init(pInfo);
        arrowGO.transform.forward = shootDirection;
        //arrowGO.St
        Rigidbody arrowRigidbody = arrowGO.GetComponent<Rigidbody>();
        arrowRigidbody.velocity = shootDirection * arrowSpeed;

        //����ͬ��
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

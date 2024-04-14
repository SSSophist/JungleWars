using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using Mirror;

//��Ʒ������
public class ItemManager : MonoBehaviour
{
    [FoldoutGroup("Set")] int targetRes = 24;

    [FoldoutGroup("Set")][LabelText("״̬")] public State state;
    [FoldoutGroup("Set")][LabelText("��ҿ�����")] public HunterController pc;
    [FoldoutGroup("Set")][Header("�������")] public GameObject computePanel;
    [FoldoutGroup("Set")][Header("�������")] Tween computePanelTween;
    [FoldoutGroup("Set")][Header("��ʼ�������ֶ���")] public TweenSettings startCompeteTS;
    [FoldoutGroup("Set")][Header("������嶯��")] public TweenSettings computePanelTS;
    [FoldoutGroup("Set")][Header("�ƶ���������")] public TweenSettings moveTS;
    [FoldoutGroup("Set")][Header("������Text")] public TMP_Text resText;
    [FoldoutGroup("Set")][Header("������RT")] public RectTransform resRT;
    [FoldoutGroup("Set")][Header("����������")] public TweenSettings resTS;
    [FoldoutGroup("Set")][LabelText("����Ԥ����")] public SymbolItem symbolPrefab;
    [FoldoutGroup("Set")][LabelText("���")] public Transform symbolContainer;
    [FoldoutGroup("Set")][Header("��ƷԤ����")] public GameObject itemUIPrefab;
    [FoldoutGroup("Set")][Header("��Ʒλ��")] public RectTransform[] itemSlotRTs;
    [FoldoutGroup("Set")][Header("����������λ��")] public RectTransform[] toBeCalculatedItemUIContainer;
    [FoldoutGroup("Set")][Header("���������λ��")] public RectTransform[] toBeCalculatedItemSymbolContainer;

    [FoldoutGroup("State")][LabelText("���")] public int res;
    [FoldoutGroup("State")][Header("��ǰ��ƷUI")] public List<ItemUI> itemUIs = new();

    [FoldoutGroup("State")][Header("����������")] public List<ItemUI> toBeCalculatedItemUIs = new();
    [FoldoutGroup("State")][Header("���������")] public List<SymbolItem> toBeCalculatedItemSymbols = new();
    public int curItemCount=> toBeCalculatedItemUIs.Count;
    public int curSymbolCount => toBeCalculatedItemSymbols.Count;
    [FoldoutGroup("End")][Header("��Ϸʤ�����")] public GameObject winPanel;
    [FoldoutGroup("End")][Header("����������")] public GameObject completeFailPanel;
    [FoldoutGroup("End")][Header("�������������")] public TMP_Text completeResText;
    [FoldoutGroup("End")][Header("��Ϸʧ�����")] public GameObject losePanel;
    public static ItemManager st;
    public enum State
    {
        Collect,       //�ռ���
        Compute,    //������
        End         //��Ϸ����
    }
    void Start()
    {
        st = this;
    }

    //����UI
    public void UpdateUI(SyncList<ItemInfo> itemInfos)
    {
        for(int i=0;i< itemUIs.Count; i++)
        {
            Destroy(itemUIs[i].gameObject);
        }

        itemUIs.Clear();

        for (int i = 0; i < itemInfos.Count; i++)
        {
            ItemUI itemUI = Instantiate(itemUIPrefab, itemSlotRTs[i].transform).GetComponent<ItemUI>();
            itemUI.Init(itemInfos[i].num, i);
            itemUIs.Add(itemUI);
        }
        Sequence seq = Sequence.Create()
            .Chain(Tween.Scale(itemUIs[itemUIs.Count - 1].transform, Vector3.one*0.2f, resTS))
            .Chain(Tween.Scale(itemUIs[itemUIs.Count - 1].transform, Vector3.one, 0.9f, Ease.OutSine));

        //�����������4����ʼ����
        if (itemInfos.Count>=4)
        {
            StartCompute();
        }
    }

    //������ִ���
    public void OnClickItem(ItemUI itemUI)
    {
        //��ֹ����ģʽ���
        if (state != State.Compute)
            return;
    
        itemUI.moveTween.Stop();
        if (itemUI.state == ItemUIState.None)
        {
            if (curItemCount >= 4)
                return;
            itemUI.moveTween = Tween.Position(itemUI.RT, toBeCalculatedItemUIContainer[curItemCount].position, moveTS);
            itemUI.state = ItemUIState.isComputing;
            toBeCalculatedItemUIs.Add(itemUI);
        }
        else if(itemUI.state == ItemUIState.isComputing)
        {
            itemUI.moveTween = Tween.Position(itemUI.RT, itemSlotRTs[itemUI.index].position, moveTS);
            itemUI.state = ItemUIState.None;
            toBeCalculatedItemUIs.Remove(itemUI);
        }

        CheckCanComplete(); 

    }
    public enum BlockState
    {
        None
    }
    public List<bool> symbolBlockState = new();
    //������Ŵ���
    public void OnClickSymbolItem(SymbolItem itemUI)
    {
        if (state != State.Compute)
            return;

        itemUI.moveTween.Stop();

        if (itemUI.state == SymbolState.None)
        {
            if (curSymbolCount >= 3)//������Χ
                return;

            SymbolItem symbolItem = Instantiate(symbolPrefab, itemUI.transform.position,Quaternion.identity);
            symbolItem.transform.SetParent(itemUI.transform.parent);

            symbolItem.Set(itemUI.symbol);
            symbolItem.index = curSymbolCount;
            //�ƶ�����
            symbolItem.moveTween = Tween.Position(symbolItem.RT, toBeCalculatedItemSymbolContainer[GetFirstEmptyIndex()].position, moveTS);//UIAnchored
            symbolItem.state = SymbolState.isComputing;
            toBeCalculatedItemSymbols.Add(symbolItem);
        }
        else if (itemUI.state == SymbolState.isComputing)
        {
            /*
            itemUI.moveTween = Tween.UIAnchoredPosition(itemUI.RT, itemUI.originPos, moveTS);
            itemUI.state = SymbolState.None;*/   
            toBeCalculatedItemSymbols.Remove(itemUI);
            Destroy(itemUI.gameObject);
        }

        // ����Ƿ����
        CheckCanComplete();
    }
    public int GetFirstEmptyIndex()
    {
        for(int i=0;i<= symbolBlockState.Count; i++)
        {
            if (symbolBlockState[i] == false)
            {
                return i;
            }
        }
        return -1;
    }
    // ����Ƿ����
    void CheckCanComplete()
    {
        // ������
        res = CalculateResult();
        resText.text = res.ToString();
        completeResText.text = "��Ĵ𰸣�" + res.ToString();
        if (toBeCalculatedItemUIs.Count >= 4 && toBeCalculatedItemSymbols.Count>=3)
        {
            //�����
            if (res == targetRes)
            {
                state = State.End;
                GameManager.st.PlayerWins();
            }
            else
            {
                state = State.Collect;
                CompeteFail();
            }
            Tween.Scale(resRT, 1.4f, resTS);
        }
    }
    //����� ��ʼ����
    public void StartCompute()
    {
        pc.canControl = false;
        state = State.Compute;
        computePanel.SetActive(true);
        computePanelTween.Stop();
        computePanelTween = Tween.Scale(computePanel.transform, Vector3.one, computePanelTS);
    }

    //ֹͣ����
    [Button("�ر����")]
    public void StopCompete()
    {
        for (int i = 0; i < itemUIs.Count; i++)
        {
            Destroy(itemUIs[i].gameObject);
        }
        for (int i = 0; i< toBeCalculatedItemUIs.Count;i++)
        {
            Destroy(toBeCalculatedItemUIs[i].gameObject);
        }
        for (int i = 0; i < toBeCalculatedItemSymbols.Count; i++)
        {
            Destroy(toBeCalculatedItemSymbols[i].gameObject);
        }


        //�����Ʒ��
        itemUIs.Clear();
        toBeCalculatedItemUIs.Clear();
        toBeCalculatedItemSymbols.Clear();
        pc.items.Clear();
        pc.canControl = true;

        resText.text = "";
        state = State.Collect;

        computePanelTween.Stop();
        computePanelTween = Tween.Scale(computePanel.transform, Vector3.zero, computePanelTS).OnComplete(()=> computePanel.SetActive(false));
    }

    // ������
    public int CalculateResult()
    {
        float result = 0;
        // ����ʹ�õ�һ��������Ϊ��ʼֵ
        if(toBeCalculatedItemUIs.Count > 0)
            result = toBeCalculatedItemUIs[0].num;

        // ���������б�
        for (int i = 0; i < toBeCalculatedItemSymbols.Count; i++)
        {
            //���ֲ���
            if((i + 1) >= toBeCalculatedItemUIs.Count)
            {
                break;
            }
            // ���ݷ��Ž�����Ӧ�ļ���
            switch (toBeCalculatedItemSymbols[i].symbol)
            {
                case "+":
                    result += toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "-":
                    result -= toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "��":
                    result *= toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "/":
                    // �������Ƿ�Ϊ��
                    if (toBeCalculatedItemUIs[i + 1].num != 0)
                    {
                        result /= toBeCalculatedItemUIs[i + 1].num;
                    }
                    else
                    {
                        Debug.Log("��������Ϊ�㣡");
                        return int.MinValue; // ����һ����ʾ�����ֵ
                    }
                    break;
                default:
                    Debug.Log("δ֪�ķ��ţ�" + toBeCalculatedItemSymbols[i].symbol);
                    return int.MinValue; // ����һ����ʾ�����ֵ
            }
        }

        return (int)result;
    }
    // ����ɹ� ʤ��
    public void Win()
    {
        //ʤ��
        TimeManager.st.CmdStopTime();
        winPanel.SetActive(true);
    }
    // ������� ʧ��
    public void CompeteFail()
    {
        // ����ʧ�ܣ�����Ѱ������
        completeFailPanel.SetActive(true);
        Tween.Delay(4f, () => completeFailPanel.SetActive(false));
        //�ر���� ֹͣ����
        StopCompete();
    }
    // ʧ�ܶ���Ч��
    public TweenSettings<Vector3> loseTS;
    public void Lose()
    {
        // ʧ�ܣ���������������
        losePanel.SetActive(true);
        Tween.Scale(losePanel.transform, loseTS).OnComplete(() => StopCompete());
    }
}


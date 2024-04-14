using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;

//物品管理器
public class ItemManager : MonoBehaviour
{
    [FoldoutGroup("Set")] public State state;
    [FoldoutGroup("Set")] public HunterController pc;
    [FoldoutGroup("Set")][Header("计算面板")] public GameObject computePanel;
    [FoldoutGroup("Set")][Header("计算面板")] Tween computePanelTween;
    [FoldoutGroup("Set")][Header("开始计算文字动画")] public TweenSettings startCompeteTS;
    [FoldoutGroup("Set")][Header("计算面板动画")] public TweenSettings computePanelTS;
    [FoldoutGroup("Set")][Header("移动动画设置")] public TweenSettings moveTS;
    [FoldoutGroup("Set")][Header("计算结果Text")] public TMP_Text resText;
    [FoldoutGroup("Set")][Header("计算结果RT")] public RectTransform resRT;
    [FoldoutGroup("Set")][Header("计算结果动画")] public TweenSettings resTS;
    [FoldoutGroup("Set")] public SymbolItem symbolPrefab;
    [FoldoutGroup("Set")] public Transform symbolContainer;

    [Header("当前物品UI")] public List<ItemUI> itemUIs = new();
    [Header("物品预制体")] public GameObject itemUIPrefab;
    [Header("物品位置")] public RectTransform[] itemSlotRTs;

    [Header("待计算数字位置")] public RectTransform[] toBeCalculatedItemUIContainer;
    [Header("待计算符号位置")] public RectTransform[] toBeCalculatedItemSymbolContainer;

    [Header("待计算数字")] public List<ItemUI> toBeCalculatedItemUIs = new();
    [Header("待计算符号")] public List<SymbolItem> toBeCalculatedItemSymbols = new();
    public int curItemCount=> toBeCalculatedItemUIs.Count;
    public int curSymbolCount => toBeCalculatedItemSymbols.Count;

    public int res;
    int targetRes = 24;

    [FoldoutGroup("End")][Header("游戏胜利面板")] public GameObject winPanel;
    [FoldoutGroup("End")][Header("计算失败面板")] public GameObject completeFailPanel;
    [FoldoutGroup("End")][Header("计算失败结果文字")] public TMP_Text completeResText;
    [FoldoutGroup("End")][Header("游戏失败面板")] public GameObject losePanel;
    public static ItemManager st;

    public enum State
    {
        None,
        Compute,
        End
    }
    private void Start()
    {
        st = this;
    }

    //更新UI
    public void UpdateUI(List<ItemInfo> itemInfos)
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
            .Chain(Tween.Scale(itemUIs[itemUIs.Count-1].transform, Vector3.one*1.34f, resTS))
            .Chain(Tween.Scale(itemUIs[itemUIs.Count - 1].transform, Vector3.one, 0.1f,Ease.Default));

        if (itemInfos.Count>=4)
        {
            StartCompute();
        }
    }

    //点击数字触发
    public void OnClickItem(ItemUI itemUI)
    {
        //防止正常模式点击
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

    //点击符号触发
    public void OnClickSymbolItem(SymbolItem itemUI)
    {
        if (state != State.Compute)
            return;

        itemUI.moveTween.Stop();
        if (itemUI.state == SymbolState.None)
        {
            if (curSymbolCount >= 3)//超过范围
                return;

            SymbolItem symbolItem = Instantiate(symbolPrefab, itemUI.transform.position,Quaternion.identity);
            symbolItem.transform.SetParent(itemUI.transform.parent);
            symbolItem.Set(itemUI.symbol);
            symbolItem.moveTween = Tween.Position(symbolItem.RT, toBeCalculatedItemSymbolContainer[curSymbolCount].position, moveTS);//UIAnchored
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

        CheckCanComplete();
    }

    // 检查是否完成
    void CheckCanComplete()
    {
        // 计算结果
        res = CalculateResult();
        resText.text = res.ToString();
        completeResText.text = "你的答案：" + res.ToString();
        if (toBeCalculatedItemUIs.Count >= 4 && toBeCalculatedItemSymbols.Count>=3)
        {
            Tween.Scale(resRT, 1.4f, resTS);
            if (res == targetRes)
            {
                state = State.End;
                GameManager.st.PlayerWins();
            }
            else
            {
                state = State.None;
                CompeteFail();
            }
           
        }
    }

    //停止计算
    [Button("关闭面板")]
    public void StopCompete()
    {
        for(int i = 0; i< toBeCalculatedItemUIs.Count;i++)
        {
            Destroy(toBeCalculatedItemUIs[i].gameObject);
        }
        for (int i = 0; i < toBeCalculatedItemSymbols.Count; i++)
        {
            Destroy(toBeCalculatedItemSymbols[i].gameObject);
        }
        //清空物品栏
        itemUIs.Clear();
        toBeCalculatedItemUIs.Clear();
        toBeCalculatedItemSymbols.Clear();

        resText.text = "";
        if(pc!=null)
            pc.canControl = true;
        state = State.None;

        computePanelTween.Stop();
        computePanelTween = Tween.Scale(computePanel.transform, Vector3.zero, computePanelTS).OnComplete(()=> computePanel.SetActive(false));
    }
    [Button("打开面板")]
    //打开面板 开始计算
    public void StartCompute()
    {
        if (pc != null)
            pc.canControl = false;
        state = State.Compute;

        computePanel.SetActive(true);
        computePanelTween.Stop();
        computePanelTween = Tween.Scale(computePanel.transform, Vector3.one, computePanelTS);
    }

    // 计算结果
    public int CalculateResult()
    {
        float result = 0;
        // 首先使用第一个数字作为初始值
        if(toBeCalculatedItemUIs.Count > 0)
            result = toBeCalculatedItemUIs[0].num;

        // 迭代符号列表
        for (int i = 0; i < toBeCalculatedItemSymbols.Count; i++)
        {
            //数字不够
            if((i + 1) >= toBeCalculatedItemUIs.Count)
            {
                break;
            }
            // 根据符号进行相应的计算
            switch (toBeCalculatedItemSymbols[i].symbol)
            {
                case "+":
                    result += toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "-":
                    result -= toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "×":
                    result *= toBeCalculatedItemUIs[i + 1].num;
                    break;
                case "/":
                    // 检查除数是否为零
                    if (toBeCalculatedItemUIs[i + 1].num != 0)
                    {
                        result /= toBeCalculatedItemUIs[i + 1].num;
                    }
                    else
                    {
                        Debug.Log("除数不能为零！");
                        return int.MinValue; // 返回一个表示错误的值
                    }
                    break;
                default:
                    Debug.Log("未知的符号：" + toBeCalculatedItemSymbols[i].symbol);
                    return int.MinValue; // 返回一个表示错误的值
            }
        }

        return (int)result;
    }
    // 计算成功 胜利
    public void Win()
    {
        //胜利
        TimeManager.st.CmdStopTime();
        winPanel.SetActive(true);
    }
    // 计算错误 失败
    public void CompeteFail()
    {
        // 计算失败，继续寻找数字
        completeFailPanel.SetActive(true);
        Tween.Delay(4f, () => completeFailPanel.SetActive(false));
        //关闭面板 停止计算
        StopCompete();
    }
    // 失败动画效果
    public TweenSettings<Vector3> loseTS;
    public void Lose()
    {
        // 失败，对手率先算出结果
        losePanel.SetActive(true);
        Tween.Scale(losePanel.transform, loseTS).OnComplete(() => StopCompete());
    }
}


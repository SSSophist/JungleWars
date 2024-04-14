using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Sirenix.OdinInspector;

public class ItemUI : MonoBehaviour
{
    [Header("��Ϣ����")] public int num;
    [Header("λ�����")] public int index;
    [Header("״̬")] public ItemUIState state;


    [FoldoutGroup("����")] public TweenSettings ts;
    [FoldoutGroup("����")] public float endScaleValue = 1.2f;

    public Tween tween;
    public Tween moveTween;
    public Sequence seq;
    [FoldoutGroup("Set")] public Text numText;
    [FoldoutGroup("Set")] public RectTransform RT;

    [Button]
    public void Init(int num,int index)
    {
        this.num = num;
        this.index = index;
        numText.text = num.ToString();
    }

    public void OnClick()
    {
        ItemManager.st.OnClickItem(this);
    }
    public void OnEnter()
    {
        if (ItemManager.st.state != ItemManager.State.Compute)
            return;
        tween.Stop();
        tween = Tween.Scale(transform, Vector3.one * endScaleValue, ts);
    }
    public void OnExit()
    {
        tween.Stop();

        if (transform.localScale == Vector3.one)
            return;
        tween = Tween.Scale(transform, Vector3.one, ts);
    }
}

public enum ItemUIState
{
    None,
    isComputing,
    EndCompute,
}
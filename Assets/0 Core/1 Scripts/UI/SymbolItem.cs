using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;
using Sirenix.OdinInspector;

public class SymbolItem : MonoBehaviour
{
    public string symbol;
    public SymbolState state;
    public int index = -1;
    [FoldoutGroup("Set")] public TMP_Text text;
    [FoldoutGroup("Set")] public RectTransform RT;
    [FoldoutGroup("¶¯»­")] public TweenSettings ts;
    [FoldoutGroup("¶¯»­")] public float endScaleValue = 1.2f;
    Tween tween;
    public Tween moveTween;

    [Button]
    public void Set(string symbol)
    {
        this.symbol = symbol;
        text.text = symbol;
    }
    public void OnDown()
    {
        ItemManager.st.OnClickSymbolItem(this);
    }
    public void OnEnter()
    {
        tween.Stop();
        tween = Tween.Scale(transform, Vector3.one * endScaleValue, ts);
    }
    public void OnExit()
    {
        tween.Stop();
        tween = Tween.Scale(transform, Vector3.one, ts);
    }
}

public enum SymbolState
{
    None,
    isComputing,
}

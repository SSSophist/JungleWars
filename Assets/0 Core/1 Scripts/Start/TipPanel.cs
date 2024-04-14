using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Sirenix.OdinInspector;

public class TipPanel : MonoBehaviour
{
    public Text tipText;
    public RectTransform tipRT;
    public float showTime = 1;
    public string message = null;

    public static TipPanel st;

    Tween tween;
    Sequence seq;
    public TweenSettings<Vector3> tipRTScale_ts;
    private void Awake()
    {
        st = this;
    }
    [Button]
    public void Show(string content)
    {
        tipText.text = content;
        tipText.CrossFadeAlpha(1, 0, false);

        //放大动画，移动动画
        seq.Stop();
        seq = Sequence.Create();
        seq.Chain(Tween.Scale(tipRT, tipRTScale_ts))
            .ChainCallback(() => tipText.CrossFadeAlpha(0, 1, false))
            .ChainDelay(1f)
            .OnComplete(()=> tipRT.gameObject.SetActive(false));
    }
}

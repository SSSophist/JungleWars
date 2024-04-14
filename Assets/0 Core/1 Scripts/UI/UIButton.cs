using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Sirenix.OdinInspector;

//UI 按钮
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IMoveHandler, IPointerMoveHandler
{
    //事件
    [FoldoutGroup("Unity Event")] public UnityEvent PointerEnter;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerExit;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerDown;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerUp;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerClick;
    [FoldoutGroup("Unity Event")] public UnityEvent<Vector3> PointerRightClick;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerClickDouble;
    [FoldoutGroup("Unity Event")] public UnityEvent PointerMove;
    [FoldoutGroup("Event")] public string eventName;
    [FoldoutGroup("Params")] public AudioSource audioSource;
    [FoldoutGroup("Params")] public AudioClip audioClick;                        //按下的音效

    public virtual void OnPointerEnter(PointerEventData d)
    {
        PointerEnter.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData d)
    {
        PointerExit.Invoke();
    }
    public virtual void OnDrag(PointerEventData d)
    {
        PointerExit.Invoke();
    }
    public virtual void OnPointerDown(PointerEventData d)
    {
        //audioSource?.PlayOneShot(audioClick);
        PointerDown.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData d)
    {
        PointerUp.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData d)
    {
        if (d.button == PointerEventData.InputButton.Right)
            PointerRightClick.Invoke(d.position);
        else
            PointerClick.Invoke();
        if (d.clickCount == 2) PointerClickDouble.Invoke();

        //if ( audioClick) AudioCenter.Instance.Play(audioClick);
    }

    public virtual void OnMove(AxisEventData d)
    {
        PointerMove.Invoke();
    }
    public virtual void OnPointerMove(PointerEventData d)
    {
        PointerMove.Invoke();
    }
}

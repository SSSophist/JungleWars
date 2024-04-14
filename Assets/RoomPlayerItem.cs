using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoomPlayerItem : MonoBehaviour
{
    public TMP_Text indexText;
    public TMP_Text nameText;
    public TMP_Text readyText;
    public void UpdateInfo(int index, string name, bool isReady)
    {
        indexText.text = index.ToString();
        nameText.text = name;
        readyText.text = isReady ? "已准备" : "未准备";
        readyText.color = isReady ? Color.green : Color.yellow;

    }
    public void UpdateReadyInfo(bool isReady)
    {
        readyText.text = isReady ? "已准备" : "未准备";
        readyText.color = isReady? Color.green : Color.yellow;
    }
}

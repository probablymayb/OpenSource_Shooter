using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI readyText;

    public void SetReady(bool isReady)
    {
        if(readyText != null)
        {
            readyText.text = isReady ? "준비 완료" : "준비 중";
            readyText.color = isReady ? Color.green : Color.red;
        }
    }
}

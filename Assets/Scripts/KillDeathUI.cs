using UnityEngine;
using TMPro;

public class KillDeathUI : MonoBehaviour
{
    public TMP_Text kdText;

    public void UpdateKD(int kills, int deaths)
    {
        kdText.text = $"Kill: {kills} / Death: {deaths}";
    }
}
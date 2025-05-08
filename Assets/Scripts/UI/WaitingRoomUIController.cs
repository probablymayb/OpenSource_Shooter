using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomUIController : MonoBehaviour
{
    [SerializeField] public Button ReadyButton;
    [SerializeField] private TextMeshProUGUI countdownText;

    public void CheckAllReady()
    {
        // 모두 준비 완료
        StartGameCountdown().Forget(); // UniTask async 호출
    }

    private async UniTaskVoid StartGameCountdown()
    {
        countdownText.gameObject.SetActive(true);

        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            await UniTask.Delay(1000);
            count--;
        }

        countdownText.text = "";
        countdownText.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Photon.StartBattle();
        }
    }
}

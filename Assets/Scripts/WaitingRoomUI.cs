using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public GameObject playerListPanel;
    public GameObject playerNameTextPrefab;

    void Start()
    {
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        // 기존 자식 삭제
        foreach (Transform child in playerListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 현재 접속한 플레이어 목록 표시
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject textObj = Instantiate(playerNameTextPrefab, playerListPanel.transform);
            textObj.GetComponent<TMP_Text>().text = player.NickName;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
}
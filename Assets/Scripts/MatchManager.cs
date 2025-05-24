using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviourPunCallbacks
{
    public GameObject popupPanel;         // 매칭 팝업
    public GameObject waitingRoomPanel;   // 대기방 UI (선택)

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void StartMatching()
    {
        popupPanel.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void CancelMatching()
    {
        popupPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); // 2인 게임 설정
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 완료!");

        popupPanel.SetActive(false);

        if (waitingRoomPanel != null)
            waitingRoomPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("플레이어 입장: " + newPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("플레이어 다 모임! 게임 시작");
            PhotonNetwork.LoadLevel("Level_00"); // 씬 이름은 정확히 등록되어 있어야 함
        }
    }
}
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviourPunCallbacks, IManager
{
    [Header("Settings")]
    public string gameVersion = "1.0";
    public string roomNamePrefix = "Room_";
    public byte maxPlayers = 2;

    private const string waitingRoomSceneName = "WaitingRoomScene";

    public static WaitingRoomPhotonManager WaitingRoom { get; } = new WaitingRoomPhotonManager();

    public void Initialize()
    {
        ConnectToPhoton();

        //포톤에서 관리될 세부 포톤 클래스 매니저들 Initialize();

        var photonManagers = new List<IManager>
        {
            WaitingRoom,
        };

        foreach(var manager in photonManagers)
        {
            manager.Initialize();
        }
    }

    public void Release()
    {

    }

    /// <summary>
    /// 인터넷 연결 + 포톤 마스터 서버 접속
    /// </summary>
    public void ConnectToPhoton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Photon 서버 연결 시도 중...");
    }

    /// <summary>
    /// 포톤 서버랑 연결되면 콜백 호출되는 함수
    /// 매칭 준비 단계
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Photon 마스터 서버 연결됨");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료");
        // UI에서 매칭 버튼 활성화 등 처리
    }

    /// <summary>
    /// 매칭 시작 버튼 클릭 시 호출
    /// </summary>
    public void StartRandomMatch()
    {
        Debug.Log("랜덤 매칭 시도 중...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("빈 룸 없음 → 새 룸 생성");
        string roomName = $"Room_{Random.Range(1000, 9999)}";
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"룸 생성 성공: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸 입장 성공");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            // 이미 다른 사람이 있어 바로 매칭 완료 → 대기방 씬 이동
            PhotonNetwork.LoadLevel(waitingRoomSceneName);
        }
        else
        {
            // 1명만 입장했을 경우 → 대기
            Debug.Log("상대방 기다리는 중...");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장 완료");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            PhotonNetwork.LoadLevel(waitingRoomSceneName);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Photon 연결 끊김: {cause}");
    }
}

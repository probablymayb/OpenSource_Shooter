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

    private const string _waitingRoomSceneName = "WaitingRoomScene";
    private const string _IngameSceneName = "InGameScene";
    public enum GamePhase { None, Matching, Waiting, InGame};
    public static GamePhase _currentPhase = GamePhase.None;

    public static WaitingRoomPhotonManager WaitingRoom { get; } = new WaitingRoomPhotonManager();
    public static InGamePhotonManager InGame { get; } = new InGamePhotonManager();

    public void Initialize()
    {
        ConnectToPhoton();

        //포톤에서 관리될 세부 포톤 클래스 매니저들 Initialize();

        var photonManagers = new List<IScenePhotonManager>
        {
            WaitingRoom, InGame,
        };

        foreach(var manager in photonManagers)
        {
            manager.Initialize();
        }
    }

    public void Release()
    {
        OnDisconnected(DisconnectCause.ApplicationQuit);
    }

    #region 포톤 서버 연결 및 로비 입장
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
    #endregion

    #region Matching
    /// <summary>
    /// 매칭 시작 버튼 클릭 시 호출
    /// </summary>
    public void StartRandomMatch()
    {
        Debug.Log("랜덤 매칭 시도 중...");
        PhotonNetwork.JoinRandomRoom();
        _currentPhase = GamePhase.Matching;
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
    #endregion

    #region Waiting 상태
    public override void OnJoinedRoom()
    {
        Debug.Log("룸 입장 성공");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            // 이미 다른 사람이 있어 바로 매칭 완료 → 대기방 씬 이동
            _currentPhase = GamePhase.InGame;
            PhotonNetwork.LoadLevel(_waitingRoomSceneName);
        }
        else
        {
            // 1명만 입장했을 경우 → 대기
            _currentPhase = GamePhase.Waiting;
            Debug.Log("상대방 기다리는 중...");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장 완료");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            _currentPhase = GamePhase.InGame;
            PhotonNetwork.LoadLevel(_waitingRoomSceneName);
        }
    }
    #endregion

    public void StartBattle()
    {
        _currentPhase = GamePhase.InGame;
        PhotonNetwork.LoadLevel(_IngameSceneName);

        Debug.Log("1:1 배틀 시작");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Photon 연결 끊김: {cause}");
    }
}

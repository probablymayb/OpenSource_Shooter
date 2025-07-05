﻿using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public enum GamePhase { None, ServerReady, Lobby, Matching, Waiting, InGame };

public class PhotonManager : MonoBehaviourPunCallbacks, IManager
{
    [Header("Settings")]
    public string gameVersion = "1.0";
    public string roomNamePrefix = "Room_";
    public byte maxPlayers = 2;

    private const string _waitingRoomSceneName = "WaitingRoomScene";
    private const string _IngameSceneName = "InGameScene";

    public static GamePhase _currentPhase = GamePhase.None;



    public void Initialize()
    {
        ConnectToPhoton();
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
        SetPhase(GamePhase.ServerReady);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료");
        SetPhase(GamePhase.Lobby);
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
        SetPhase(GamePhase.Matching);
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
            SetPhase(GamePhase.InGame);
            PhotonNetwork.LoadLevel(_waitingRoomSceneName);
        }
        else
        {
            // 1명만 입장했을 경우 → 대기
            SetPhase(GamePhase.Waiting);
            Debug.Log("상대방 기다리는 중...");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장 완료");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            SetPhase(GamePhase.InGame);
            PhotonNetwork.LoadLevel(_waitingRoomSceneName);
        }
    }
    #endregion

    public void StartBattle()
    {
        SetPhase(GamePhase.InGame);
        PhotonNetwork.LoadLevel(_IngameSceneName);

        Debug.Log("1:1 배틀 시작");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Photon 연결 끊김: {cause}");
    }

    private static void SetPhase(GamePhase phase)
    {
        _currentPhase = phase;

        var matchingPresenter = GameManager.UI.GetScenePresenter<MatchingPopupUIPresenter>("StartUI");

        if( matchingPresenter != null )
        {
            Debug.Log($"GamePhase 변경 => {phase}");
            Debug.Log($"{matchingPresenter.ToString()}에 값 전송");
            matchingPresenter.OnPhaseChanged(phase);
        }
        else
        {
            Debug.LogError($"Presenter MatchingPopupUIPresenter를 찾을 수 없음");
        }
        
    }
}

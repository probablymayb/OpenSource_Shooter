using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;

public class InGamePhotonManager : MonoBehaviourPunCallbacks, IScenePhotonManager
{
    [SerializeField] private GameObject mapRoot;

    private Dictionary<int, GameObject> _avatarMap = new Dictionary<int, GameObject>();

    public static InGamePhotonManager InGame { get; private set; }

    private void Awake()
    {
        if (InGame == null) InGame = this;
        else Destroy(gameObject); // 중복 방지

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            InitializeSceneObject();
        }
    }

    private void OnDestroy()
    {
        if (InGame == this)
        {
            InGame = null;
        }
    }

    public void InitializeSceneObject()
    {
        Vector3 spawnPos = GetSpawnPosition(); // 위치 설정
        GameObject avatar = PhotonNetwork.Instantiate("Player/Player", spawnPos, Quaternion.identity);
        avatar.name = $"Avatar_{PhotonNetwork.LocalPlayer.UserId}";

        _avatarMap[PhotonNetwork.LocalPlayer.ActorNumber] = avatar;

        bool isLaterJoiner = false;
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        foreach (var other in PhotonNetwork.PlayerListOthers)
        {
            if (other.ActorNumber < myActorNumber)
            {
                isLaterJoiner = true;
                break;
            }
        }

        if (isLaterJoiner)
        {
            avatar.GetComponent<PlayerBodyPartsHandler>()
                  .SetBodyPartsDirection(PlayerBodyPartsHandler.Direction.Left);
        }
        else
        {
            avatar.GetComponent<PlayerBodyPartsHandler>()
                  .SetBodyPartsDirection(PlayerBodyPartsHandler.Direction.Right);
        }
    }


    public void UpdatePlayerAvatar(Player player)
    {

    }

    /// <summary>
    /// 기본 스폰 위치 설정 (예시)
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        // 나의 ActorNumber
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // 방에 있는 모든 플레이어 중 나보다 먼저 들어온 사람이 있는지 확인
        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            if (player.ActorNumber < myActorNumber)
            {
                // 내가 나중에 들어온 플레이어면 오른쪽에 스폰
                return new Vector3(5f, 0, 0);
            }
        }

        // 내가 가장 먼저 들어왔거나 혼자 있을 경우 왼쪽에 스폰
        return new Vector3(-5f, 0, 0);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (_avatarMap.TryGetValue(otherPlayer.ActorNumber, out var avatar))
        {
            Destroy(avatar);
            _avatarMap.Remove(otherPlayer.ActorNumber);
        }
    }

    public GameObject GetMyAvatar()
    {
        foreach(var avatar in _avatarMap.Values)
        {
            if(avatar.TryGetComponent(out PhotonView pv) && pv.IsMine)
            {
                Debug.Log("내 아바타 발견: " + avatar);
                return avatar;
            }
        }

        Debug.LogWarning("내 아바타를 발견하지 못했습니다.");
        return null;
    }
}

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;

public class InGamePhotonManager : MonoBehaviourPunCallbacks, IScenePhotonManager
{
    [SerializeField] private GameObject mapRoot;
    private Dictionary<int, GameObject> _avatarMap = new Dictionary<int, GameObject>();

    public void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            InitializeSceneObject();
        }
    }

    public void Initialize()
    {
        
    }

    public void Release()
    {

    }

    public void InitializeSceneObject()
    {
        Vector3 spawnPos = GetSpawnPosition(); // 스폰 위치 설정 함수 필요
        GameObject avatar = PhotonNetwork.Instantiate("Player/Player", spawnPos, Quaternion.identity);
        avatar.name = $"Avatar_{PhotonNetwork.LocalPlayer.UserId}";

        _avatarMap[PhotonNetwork.LocalPlayer.ActorNumber] = avatar;

        // 방향 반전: 예를 들어 ActorNumber가 짝수면 좌향, 홀수면 우향
        if (PhotonNetwork.LocalPlayer.ActorNumber % 2 != 0)
        {
            var scale = avatar.transform.localScale;
            avatar.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
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
        // 임의로 서로 다른 위치로 스폰하도록 설정
        if(PhotonNetwork.LocalPlayer.ActorNumber % 2 != 0)
        {
            return new Vector3(5f, 0, 0);
        }

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
}

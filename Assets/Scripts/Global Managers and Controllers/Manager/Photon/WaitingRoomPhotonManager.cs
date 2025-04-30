using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Photon 대기방에서 아바타 생성 및 준비 상태 동기화를 관리하는 매니저 클래스
/// </summary>
public class WaitingRoomPhotonManager : MonoBehaviourPunCallbacks, IManager
{
    [SerializeField] private WaitingRoomUIController waitingRoomUIController;
    [SerializeField] private Transform myAvatarParent;
    [SerializeField] private Transform opponentAvatarParent;

    private Dictionary<int, GameObject> _avatarMap = new Dictionary<int, GameObject>();
    private const string READY_KEY = "IsReady";
    private bool _isReady = false;

    /// <summary>
    /// MonoBehaviour Awake: 버튼 리스너 등록 및 초기 아바타 생성
    /// </summary>
    public void Awake()
    {
        waitingRoomUIController.ReadyButton.onClick.AddListener(OnReadyClicked);
        InitializeAllPlayers();
    }

    public void Initialize()
    {
        
    }

    /// <summary>
    /// 현재 룸에 있는 모든 플레이어에 대해 아바타 생성
    /// </summary>
    void InitializeAllPlayers()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            CreateAvatar(player);
        }
    }

    /// <summary>
    /// 비동기 아바타 생성 (프리팹 Addressables 로드)
    /// </summary>
    /// <param name="player">포톤서버의 플레이어에서 가져온 플레이어</param>
    private async void CreateAvatar(Player player)
    {
        Transform parent = player.IsLocal ? myAvatarParent : opponentAvatarParent;

        GameObject avatar = await GameManager.Resource.Instantiate("PlayerInfo", parent);
        avatar.name = $"Avatar_{player.UserId}";
        _avatarMap[player.ActorNumber] = avatar;

        if (!player.IsLocal)
        {
            var rt = avatar.GetComponent<Transform>();
            if(rt != null)
            {
                rt.localScale = new Vector3(-rt.localScale.x, rt.localScale.y, rt.localScale.z);
            }
        }

        var info = avatar.GetComponent<PlayerInfoUIController>();
        if (info != null)
        {
            info.SetReady(false);
        }
    }

    public void Release()
    {

    }

    /// <summary>
    /// Ready 버튼 클릭 시 호출되는 메서드
    /// </summary>
    public void OnReadyClicked()
    {
        _isReady = true;

        // 내 상태 저장
        var props = new Hashtable { { READY_KEY, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    /// <summary>
    /// Photon 플레이어의 커스텀 프로퍼티가 변경되었을 때 호출됨
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(READY_KEY))
        {
            UpdatePlayerAvatar(targetPlayer);

            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (!player.CustomProperties.ContainsKey(READY_KEY) || !(bool)player.CustomProperties[READY_KEY])
                    return;
            }
            waitingRoomUIController.CheckAllReady();
        }
    }

    void UpdatePlayerAvatar(Player player)
    {
        Transform parent = player.IsLocal ? myAvatarParent : opponentAvatarParent;

        var info = parent.gameObject.GetComponent<PlayerInfoUIController>();
        if (info != null)
        {
            bool isReady = player.CustomProperties.ContainsKey(READY_KEY) && (bool)player.CustomProperties[READY_KEY];
            info.SetReady(isReady);
        }
    }
}

using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// This class receives inputs from TadaInput and calls methods from other classes based on those inputs. It 
/// also handles movement.
/// </summary>
public class PlayerStatus : MonoBehaviourPunCallbacks
{
    [Header("Health")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    // UI 연동을 위한 참조 (추가된 부분)
    private PlayerHUD playerHUD;

    // UI 연동을 위한 프로퍼티들 (추가된 부분)
    public int CurrentHP => currentHp;
    public int MaxHP => maxHp;
    public float HealthRatio => maxHp > 0 ? (float)currentHp / maxHp : 0f;
    public bool IsDead => currentHp <= 0;

    void Start()
    {
        currentHp = maxHp;

        // UI 참조 찾기 (추가된 부분)
        playerHUD = GetComponent<PlayerHUD>();
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        if (currentHp <= 0) return;

        currentHp -= damage;
        photonView.RPC("SyncHp", RpcTarget.Others, currentHp);
        Debug.Log($"[{photonView.Owner.NickName}] 피해 받음: {damage}, 남은 체력: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    [PunRPC]
    public void SyncHp(int syncedHP)
    {
        currentHp = syncedHP;
    }

    private void Die()
    {
        Debug.Log($"[{photonView.Owner.NickName}] 사망!");

        transform.GetComponentInChildren<TadaInput>().enabled = false;
        playerHUD.ShowGameOver();
    }

}
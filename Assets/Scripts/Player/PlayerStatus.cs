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
        // 죽어있으면 데미지 무시
        if (currentHp <= 0) return;

        currentHp -= damage;
        Debug.Log($"[{photonView.Owner.NickName}] 피해 받음: {damage}, 남은 체력: {currentHp}/{maxHp}");

        // UI 업데이트 (체력바 등)
        UpdateHealthUI();

        // 피격 이펙트
        PlayHitEffect();

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        // 체력바 UI 업데이트
        // 자신의 캐릭터인 경우에만 UI 업데이트
        if (photonView.IsMine)
        {
            // 기존 코드 (주석 처리됨)
            // GameManager.UI.UpdateHealthBar(currentHp, maxHp);

            // 새로운 UI 시스템 (추가된 부분)
            if (playerHUD != null)
            {
                // PlayerHUD가 자동으로 업데이트하므로 별도 호출 불필요
                // playerHUD.UpdateHealthUI();
            }
        }
    }

    private void PlayHitEffect()
    {
        // 피격 애니메이션, 사운드, 파티클 등
        // 모든 클라이언트에서 보여야 함
    }

    private void Die()
    {
        Debug.Log($"[{photonView.Owner.NickName}] 사망!");

        // 사망 애니메이션
        // animator.SetTrigger("Die");

        // 움직임 비활성화
        if (photonView.IsMine)
        {
            // 입력 비활성화
            //GetComponent<TadaInput>().enabled = false;

            // 게임오버 UI 표시 (추가된 부분)
            if (playerHUD != null)
            {
                playerHUD.ShowGameOver();
            }
        }

        // 리스폰 처리 (3초 후)
        if (photonView.IsMine)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(3f);

        // 리스폰 위치로 이동
        transform.position = GetRespawnPosition();

        // 체력 회복
        currentHp = maxHp;
        UpdateHealthUI();

        // 입력 재활성화
        GetComponent<TadaInput>().enabled = true;

        // 게임오버 UI 숨기기 (추가된 부분)
        if (playerHUD != null)
        {
            playerHUD.HideGameOver();
        }

        // 다른 플레이어에게도 리스폰 알림
        photonView.RPC("OnRespawn", RpcTarget.Others);
    }

    [PunRPC]
    void OnRespawn()
    {
        // 다른 플레이어 화면에서 리스폰 처리
        // 애니메이션 초기화 등
    }

    Vector3 GetRespawnPosition()
    {
        // 리스폰 위치 반환
        return Vector3.zero; // 또는 스폰 포인트
    }

    // 테스트용 메서드들 (추가된 부분)
    void Update()
    {
        // 자신의 플레이어만 키 입력 받기
        if (photonView.IsMine)
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H))
                Heal(20); // H키로 20 체력 회복

            if (Input.GetKeyDown(KeyCode.J))
                TakeDamage(20); // J키로 20 피해
#endif
        }
    }

    // 체력 회복 메서드 (추가된 부분)
    public void Heal(int healAmount)
    {
        if (currentHp <= 0 || healAmount <= 0) return;

        currentHp = Mathf.Min(currentHp + healAmount, maxHp);
        UpdateHealthUI();

        Debug.Log($"[{photonView.Owner.NickName}] 체력 회복: {healAmount}, 현재 체력: {currentHp}/{maxHp}");
    }
}
using UnityEngine;

/// <summary>
/// 기존 무기 클래스를 수정하지 않고 UI 연동을 위한 컴포넌트
/// 기존 무기 오브젝트에 이 컴포넌트만 추가하면 자동으로 UI 업데이트
/// </summary>
public class WeaponUIUpdater : MonoBehaviour
{
    [Header("무기 설정")]
    public int maxAmmo = 30;        // 수동으로 설정
    public int currentAmmo = 30;    // 수동으로 관리

    [Header("키 입력 설정")]
    public KeyCode fireKey = KeyCode.Space;
    public KeyCode reloadKey = KeyCode.R;

    private int lastAmmo;           // 변화 감지용

    void Start()
    {
        lastAmmo = currentAmmo;
        UpdateUI();
    }

    void Update()
    {
        // 키 입력 처리
        HandleInput();

        // 탄약 변화 감지 및 UI 업데이트
        if (currentAmmo != lastAmmo)
        {
            UpdateUI();
            lastAmmo = currentAmmo;
        }
    }

    void HandleInput()
    {
        // 발사
        if (Input.GetKeyDown(fireKey))
        {
            Fire();
        }

        // 재장전
        if (Input.GetKeyDown(reloadKey))
        {
            Reload();
        }
    }

    // 발사 (public이므로 다른 스크립트에서도 호출 가능)
    public void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log($"발사! 남은 탄약: {currentAmmo}");
        }
        else
        {
            Debug.Log("탄약 없음!");
        }
    }

    // 재장전
    public void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("재장전 완료!");
    }

    // UI 업데이트
    void UpdateUI()
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        }
    }

    // 외부에서 탄약 설정 (기존 무기 스크립트에서 호출 가능)
    public void SetAmmo(int current, int max = -1)
    {
        currentAmmo = current;
        if (max > 0) maxAmmo = max;
        UpdateUI();
    }
}
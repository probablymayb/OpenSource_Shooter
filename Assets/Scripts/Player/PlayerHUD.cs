using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// 각 플레이어 프리팹에 붙일 개별 HUD
/// 기존 PlayerStatus 구조와 호환되는 UI 시스템
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider hpSlider;
    public TextMeshProUGUI ammoText;
    public GameObject gameOverPanel;

    [Header("Canvas Settings")]
    public Canvas hudCanvas;

    [Header("Debug")]
    public bool showDebugLogs = true; // 기본적으로 켜두기

    // 컴포넌트 참조
    private PlayerStatus playerStatus;
    private Weapon weapon;

    // 디버깅용 변수
    private int lastAmmoCount = -1;
    private bool weaponFound = false;

    void Start()
    {
        // 같은 GameObject의 컴포넌트들 찾기
        playerStatus = GetComponent<PlayerStatus>();

        if (showDebugLogs) Debug.Log($"[{gameObject.name}] PlayerHUD Start 시작");

        // Weapon 찾기
        FindWeaponComponent();

        // 못 찾았으면 재시도 시작
        if (weapon == null)
        {
            InvokeRepeating(nameof(RetryFindWeapon), 1f, 1f);
            if (showDebugLogs) Debug.LogWarning($"[{gameObject.name}] Weapon을 찾지 못해 재시도 시작");
        }

        // Canvas 설정
        SetupCanvas();

        // 게임오버 패널 숨김
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 초기 UI 업데이트
        UpdateAllUI();

        if (showDebugLogs)
        {
            Debug.Log($"[{gameObject.name}] PlayerHUD 초기화 완료");
            Debug.Log($"PlayerStatus: {(playerStatus != null ? "OK" : "NULL")}");
            Debug.Log($"Weapon: {(weapon != null ? weapon.name : "NULL")}");
            Debug.Log($"PhotonView IsMine: {(playerStatus != null ? playerStatus.photonView.IsMine.ToString() : "NULL")}");

            if (weapon != null)
            {
                Debug.Log($"초기 탄약 상태: {weapon.CurrentAmmo}/{weapon.MaxAmmo}");
            }
        }
    }

    void Update()
    {
        // 자신의 플레이어만 UI 업데이트
        if (playerStatus != null && playerStatus.photonView.IsMine)
        {
            // Weapon 재검색 (못 찾았으면)
            if (weapon == null && !weaponFound)
            {
                FindWeaponComponent();
            }

            UpdateAllUI();

            // 디버깅: 탄약 변화 감지
            if (showDebugLogs && weapon != null)
            {
                if (weapon.CurrentAmmo != lastAmmoCount)
                {
                    Debug.Log($"[{gameObject.name}] 탄약 변화: {lastAmmoCount} → {weapon.CurrentAmmo} (최대: {weapon.MaxAmmo})");
                    lastAmmoCount = weapon.CurrentAmmo;
                }

                // 매 3초마다 현재 상태 출력
                if (Time.time % 3f < 0.1f)
                {
                    Debug.Log($"[{gameObject.name}] 현재 탄약 상태: {weapon.CurrentAmmo}/{weapon.MaxAmmo}, 재장전중: {weapon.IsReloading}");
                }
            }
        }
    }

    // Weapon 컴포넌트 찾기 (강화된 버전)
    void FindWeaponComponent()
    {
        if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 찾기 시작...");

        // 방법 1: 같은 GameObject에서 찾기
        weapon = GetComponent<Weapon>();
        if (weapon != null)
        {
            weaponFound = true;
            if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 발견 (같은 오브젝트): {weapon.name}");
            lastAmmoCount = weapon.CurrentAmmo;
            return;
        }

        // 방법 2: 자식 오브젝트에서 찾기
        weapon = GetComponentInChildren<Weapon>();
        if (weapon != null)
        {
            weaponFound = true;
            if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 발견 (자식 오브젝트): {weapon.name}");
            lastAmmoCount = weapon.CurrentAmmo;
            return;
        }

        // 방법 3: 부모 오브젝트에서 찾기
        weapon = GetComponentInParent<Weapon>();
        if (weapon != null)
        {
            weaponFound = true;
            if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 발견 (부모 오브젝트): {weapon.name}");
            lastAmmoCount = weapon.CurrentAmmo;
            return;
        }

        // 방법 4: 씬의 모든 Weapon에서 같은 플레이어 소속 찾기
        Weapon[] allWeapons = FindObjectsOfType<Weapon>();
        if (showDebugLogs) Debug.Log($"[{gameObject.name}] 씬에서 찾은 Weapon 수: {allWeapons.Length}");

        foreach (var w in allWeapons)
        {
            // 같은 PhotonView 소유자인지 확인
            var weaponPhotonView = w.GetComponent<PhotonView>() ?? w.GetComponentInParent<PhotonView>();
            if (weaponPhotonView != null && weaponPhotonView == playerStatus.photonView)
            {
                weapon = w;
                weaponFound = true;
                if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 발견 (PhotonView 매칭): {weapon.name}");
                lastAmmoCount = weapon.CurrentAmmo;
                return;
            }

            // 같은 Transform 계층에 있는지 확인
            if (w.transform.IsChildOf(transform) || w.transform == transform || transform.IsChildOf(w.transform))
            {
                weapon = w;
                weaponFound = true;
                if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 발견 (Transform 계층): {weapon.name}");
                lastAmmoCount = weapon.CurrentAmmo;
                return;
            }
        }

        // 못 찾았을 때 정보 출력
        if (showDebugLogs)
        {
            Debug.LogWarning($"[{gameObject.name}] Weapon을 찾을 수 없습니다!");
            Debug.Log($"현재 GameObject 구조:");
            PrintObjectHierarchy(transform, 0);
        }
    }

    void RetryFindWeapon()
    {
        if (weapon == null)
        {
            FindWeaponComponent();
            if (weapon != null)
            {
                CancelInvoke(nameof(RetryFindWeapon));
                if (showDebugLogs) Debug.Log($"[{gameObject.name}] Weapon 재시도 성공!");
            }
        }
        else
        {
            CancelInvoke(nameof(RetryFindWeapon));
        }
    }

    // 오브젝트 계층 구조 출력 (디버깅용)
    void PrintObjectHierarchy(Transform t, int depth)
    {
        string indent = new string(' ', depth * 2);
        var components = t.GetComponents<Component>();
        string componentList = "";
        foreach (var comp in components)
        {
            if (comp != null)
                componentList += comp.GetType().Name + " ";
        }
        Debug.Log($"{indent}{t.name} [{componentList}]");

        for (int i = 0; i < t.childCount; i++)
        {
            PrintObjectHierarchy(t.GetChild(i), depth + 1);
        }
    }

    // Canvas 설정
    void SetupCanvas()
    {
        if (hudCanvas != null)
        {
            hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            hudCanvas.worldCamera = Camera.main;
            hudCanvas.sortingOrder = 100;
        }
    }

    // 전체 UI 업데이트
    void UpdateAllUI()
    {
        UpdateHealthUI();
        UpdateAmmoUI();
    }

    // 체력 UI 업데이트
    void UpdateHealthUI()
    {
        if (hpSlider != null && playerStatus != null)
        {
            float healthRatio = playerStatus.HealthRatio;
            hpSlider.value = healthRatio;

            // 체력에 따른 색상 변경
            Image fillImage = hpSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                if (healthRatio > 0.6f)
                    fillImage.color = Color.green;
                else if (healthRatio > 0.3f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
    }

    // 탄약 UI 업데이트
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if (weapon != null)
            {
                string ammoInfo;

                if (weapon.IsReloading)
                {
                    ammoInfo = "Reloading...";
                    ammoText.color = Color.yellow;
                }
                else
                {
                    ammoInfo = $"{weapon.CurrentAmmo} / {weapon.MaxAmmo}";

                    // 탄약 부족 시 빨간색
                    if (weapon.CurrentAmmo <= 5)
                        ammoText.color = Color.red;
                    else
                        ammoText.color = Color.white;
                }

                ammoText.text = ammoInfo;

                // 디버깅: UI 업데이트 확인
                if (showDebugLogs && Time.time % 2f < 0.1f) // 2초마다
                {
                    Debug.Log($"[{gameObject.name}] UI 업데이트: {ammoInfo}, Weapon: {weapon.name}");
                }
            }
            else
            {
                // Weapon이 없으면 기본 텍스트
                ammoText.text = "No Weapon";
                ammoText.color = Color.gray;

                if (showDebugLogs && Time.time % 5f < 0.1f) // 5초마다
                {
                    Debug.LogWarning($"[{gameObject.name}] Weapon이 null입니다!");
                }
            }
        }
        else
        {
            if (showDebugLogs && Time.time % 10f < 0.1f) // 10초마다
            {
                Debug.LogWarning($"[{gameObject.name}] ammoText가 null입니다!");
            }
        }
    }

    // 게임오버 표시
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (showDebugLogs) Debug.Log($"{gameObject.name} 게임오버 표시");
        }
    }

    // 게임오버 숨기기 (리스폰 시)
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            if (showDebugLogs) Debug.Log($"{gameObject.name} 게임오버 숨김");
        }
    }

    // 수동 Weapon 설정
    public void SetWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        if (showDebugLogs) Debug.Log($"{gameObject.name}: Weapon 수동 설정 - {weapon.name}");
    }

    // 디버그 정보 출력
    [ContextMenu("Debug Info")]
    public void PrintDebugInfo()
    {
        Debug.Log($"=== {gameObject.name} PlayerHUD Debug Info ===");
        Debug.Log($"PlayerStatus: {(playerStatus != null ? "OK" : "NULL")}");
        Debug.Log($"Weapon: {(weapon != null ? weapon.name : "NULL")}");
        Debug.Log($"Weapon Found: {weaponFound}");

        if (playerStatus != null)
        {
            Debug.Log($"Is Mine: {playerStatus.photonView.IsMine}");
            Debug.Log($"Current HP: {playerStatus.CurrentHP}");
            Debug.Log($"Max HP: {playerStatus.MaxHP}");
            Debug.Log($"Health Ratio: {playerStatus.HealthRatio}");
            Debug.Log($"Is Dead: {playerStatus.IsDead}");
        }

        if (weapon != null)
        {
            Debug.Log($"Weapon Type: {weapon.GetType().Name}");
            Debug.Log($"Weapon GameObject: {weapon.gameObject.name}");
            Debug.Log($"Current Ammo: {weapon.CurrentAmmo}");
            Debug.Log($"Max Ammo: {weapon.MaxAmmo}");
            Debug.Log($"Is Reloading: {weapon.IsReloading}");
            Debug.Log($"Has Ammo: {weapon.HasAmmo}");
            Debug.Log($"Use Rate: {weapon.UseRate}");
        }
        else
        {
            Debug.Log("=== Weapon 찾기 재시도 ===");
            FindWeaponComponent();
        }

        Debug.Log($"UI Elements:");
        Debug.Log($"- HP Slider: {(hpSlider != null ? "OK" : "NULL")}");
        Debug.Log($"- Ammo Text: {(ammoText != null ? ammoText.text : "NULL")}");
        Debug.Log($"- Game Over Panel: {(gameOverPanel != null ? "OK" : "NULL")}");
        Debug.Log($"- HUD Canvas: {(hudCanvas != null ? "OK" : "NULL")}");

        Debug.Log("=== GameObject 계층 구조 ===");
        PrintObjectHierarchy(transform, 0);
    }

    // 테스트용: 수동으로 무기 발사 확인
    [ContextMenu("Test Weapon Fire")]
    public void TestWeaponFire()
    {
        if (weapon != null)
        {
            Debug.Log($"[테스트] 발사 전 탄약: {weapon.CurrentAmmo}/{weapon.MaxAmmo}");

            // 기존 Weapon의 PrimaryAction 호출
            weapon.PrimaryAction(true);

            Debug.Log($"[테스트] 발사 후 탄약: {weapon.CurrentAmmo}/{weapon.MaxAmmo}");
        }
        else
        {
            Debug.LogError("[테스트] Weapon이 null입니다!");
        }
    }

    // 테스트용: 수동으로 재장전 확인
    [ContextMenu("Test Weapon Reload")]
    public void TestWeaponReload()
    {
        if (weapon != null)
        {
            Debug.Log($"[테스트] 재장전 전 탄약: {weapon.CurrentAmmo}/{weapon.MaxAmmo}");

            weapon.Reload();

            Debug.Log($"[테스트] 재장전 후 탄약: {weapon.CurrentAmmo}/{weapon.MaxAmmo}");
        }
        else
        {
            Debug.LogError("[테스트] Weapon이 null입니다!");
        }
    }
}
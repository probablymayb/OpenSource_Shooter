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

    void Start()
    {
        // 같은 GameObject의 컴포넌트들 찾기
        playerStatus = GetComponent<PlayerStatus>();

        if (showDebugLogs) Debug.Log($"[{gameObject.name}] PlayerHUD Start 시작");

        // Canvas 설정
        SetupCanvas();

        // 게임오버 패널 숨김
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (playerStatus == null) return;

        UpdateHealthUI();
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

    // 게임오버 표시
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (showDebugLogs) Debug.Log($"{gameObject.name} 게임오버 표시");
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HUD UI 요소")]
    public Slider hpSlider;
    public TextMeshProUGUI ammoText;
    public GameObject gameOverPanel;
    public Button restartButton;

    // 싱글톤 패턴으로 어디서든 접근 가능
    public static HUDManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            });
        }
    }

    // 체력 업데이트
    public void UpdateHP(float current, float max)
    {
        if (hpSlider != null && max > 0)
        {
            hpSlider.value = current / max;
        }
    }

    // 탄약 업데이트
    public void UpdateAmmo(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{current} / {max}";
        }
    }

    // 게임오버 표시
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
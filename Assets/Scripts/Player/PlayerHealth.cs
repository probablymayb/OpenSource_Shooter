using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public GameObject endGamePanel; // ✅ Inspector에서 연결할 UI

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (endGamePanel != null)
            endGamePanel.SetActive(false); // 처음엔 꺼져 있어야 함
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // 사망 처리
        }
    }

    void Die()
    {
        isDead = true;

        // ✅ EndGame UI 보여주기
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }

        // ✅ 플레이어 조작 중지 또는 오브젝트 숨기기
        gameObject.SetActive(false);
    }
}
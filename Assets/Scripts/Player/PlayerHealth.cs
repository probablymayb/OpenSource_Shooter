using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private HealthBarUI healthUI;

    public GameObject model;   // 플레이어 외형 (사망 시 숨기기용)
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        healthUI = FindObjectOfType<HealthBarUI>();
        if (healthUI != null)
        {
            healthUI.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthUI != null)
        {
            healthUI.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("플레이어 사망!");

        if (model != null)
            model.SetActive(false);

        Invoke(nameof(Respawn), 3f);
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (model != null)
            model.SetActive(true);

        if (healthUI != null)
        {
            healthUI.SetHealth(currentHealth);
        }
    }
}
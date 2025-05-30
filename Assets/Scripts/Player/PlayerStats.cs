private int kills = 0;
private int deaths = 0;
private KillDeathUI kdUI;

void Start()
{
    kdUI = FindObjectOfType<KillDeathUI>();
    UpdateUI();
}

void UpdateUI()
{
    if (kdUI != null)
    {
        kdUI.UpdateKD(kills, deaths);
    }
}

public void AddKill()
{
    kills++;
    UpdateUI();
}

public void Die()
{
    deaths++;
    UpdateUI();
    // 기타 사망 처리
}
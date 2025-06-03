using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResultUIManager : MonoBehaviour
{
    [Header("결과 UI 구성 요소")]
    public GameObject resultPanel;              // 상단 결과 패널
    public GameObject playerResultList;         // Vertical Layout Group
    public GameObject playerEntryPrefab;        // 플레이어 이름+킬/데스 표시용 프리팹

    // 플레이어 정보 클래스
    public class PlayerResult
    {
        public string name;
        public int kills;
        public int deaths;

        public PlayerResult(string name, int kills, int deaths)
        {
            this.name = name;
            this.kills = kills;
            this.deaths = deaths;
        }
    }

    void Start()
    {
        resultPanel.SetActive(false); // 시작 시 결과 패널 숨김
    }

    void Update()
    {
        // Tab 누를 동안 표시, 떼면 숨김
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            resultPanel.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            resultPanel.SetActive(false);
        }
    }

    // 결과 표시 함수 (게임 종료 시 또는 갱신 시 호출)
    public void ShowResults(List<PlayerResult> results)
    {
        // 기존 항목 제거
        foreach (Transform child in playerResultList.transform)
        {
            Destroy(child.gameObject);
        }

        // 새로운 항목 추가
        foreach (var player in results)
        {
            GameObject entry = Instantiate(playerEntryPrefab, playerResultList.transform);
            TMP_Text text = entry.GetComponent<TMP_Text>();
            text.text = $"{player.name} - Kill: {player.kills} / Death: {player.deaths}";
        }
    }
}
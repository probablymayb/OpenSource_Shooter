using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    // 로비 씬으로 돌아가기 (씬 이름은 StartScene이라고 가정)
    public void GoToLobby()
    {
        //SceneManager.LoadScene("StartScene"); // 🔁 본인의 로비 씬 이름으로 수정
    }

    // 게임 종료 (빌드 실행 시만 동작)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료 (에디터에서는 안 꺼짐)");
    }
}
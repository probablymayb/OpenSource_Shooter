using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("Level_00");
    }
}
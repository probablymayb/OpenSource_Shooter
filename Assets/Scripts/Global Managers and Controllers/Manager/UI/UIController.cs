using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //public float Progress { get; private set; }

    private const string matchPopupName = "MatchingPopup";

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Initialize();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var popup = GameManager.UI.IsPopupStack(matchPopupName);

            if (popup != null)
            {
                GameManager.UI.HidePopup(popup);
            }
            else
            {
                GameManager.UI.ShowPopup(matchPopupName).Forget();
            }
        }
    }
}

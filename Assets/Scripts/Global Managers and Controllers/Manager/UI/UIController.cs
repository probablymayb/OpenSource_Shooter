using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private const string matchPopupName = "MatchingPopup";
    //private static UIController instance;


    void Awake()
    {
       // instance = this;
        //Debug.Log("[UIController] Awake called!");

    }

    void Start()
    {
        GameManager.Initialize();
    }

    void Update()
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
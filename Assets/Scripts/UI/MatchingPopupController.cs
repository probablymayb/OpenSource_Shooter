using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingPopupController : MonoBehaviour, IPopupPresenter
{
    [SerializeField] private Button _matchStartBtn;

    public void Awake()
    {
        _matchStartBtn.interactable = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
    }

    public virtual void ShowView()
    {
        gameObject.SetActive(true);
    }

    public virtual void HideView()
    {
        gameObject.SetActive(false);
    }

    public void OnClickMatchingStartBtn()
    {
        GameManager.Photon.StartRandomMatch();
        _matchStartBtn.interactable = false;
    }

}

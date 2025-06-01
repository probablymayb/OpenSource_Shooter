using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button startBtn;

    void Update()
    {
        startBtn.interactable = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
    }

    void Start()
    {
        GameManager.Initialize();

        startBtn.onClick.AddListener(() =>
        {
            startBtn.interactable = false;
            GameManager.Photon.StartRandomMatch();
        });
    }
}
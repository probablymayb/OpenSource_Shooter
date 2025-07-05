using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchingEvent
{
    StartMatching,
}


public class MatchingPopupUIModel : ModelBase
{
    private bool _matchBtnInteract = false;

    public bool MatchBtnInteract => _matchBtnInteract;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void OnMatchingStartBtn()
    {
        GameManager.Photon.StartRandomMatch();
        _matchBtnInteract = false;

        TriggerEvent(MatchingEvent.StartMatching, _matchBtnInteract);
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        bool canStart = (phase == GamePhase.Lobby);
        TriggerEvent(MatchingEvent.StartMatching, canStart);

        Debug.Log($"GamePhase에 따른 버튼 활성화 비활성화 변경 => 활성화: {canStart}");
    }
}

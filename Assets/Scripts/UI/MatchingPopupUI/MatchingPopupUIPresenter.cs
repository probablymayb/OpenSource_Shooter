using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchingPopupUIPresenter : PresenterBase<MatchingPopupUIModel>
{

    protected override void Initialize()
    {
        GameManager.UI.FindAllPresenter();

        base.Initialize();

        AddListener<Unit>(MatchingEvent.StartMatching, _ =>
        {
            OnClickMatchingStartBtn();
        });

        model.AddListener<bool>(MatchingEvent.StartMatching, updateInteract =>
        {
            view.UpdateView(updateInteract);
        });
    }

    public void OnClickMatchingStartBtn()
    {
        model.OnMatchingStartBtn();
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        model.OnPhaseChanged(phase);
    }

}

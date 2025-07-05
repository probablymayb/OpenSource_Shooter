using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MatchingPopupUIView : ViewBase<MatchingPopupUIModel>
{
    [SerializeField] private Button matchStartBtn;

    public override void Initialize(PresenterBase<MatchingPopupUIModel> presenter)
    {
        base.Initialize(presenter);

        AddUIListener<Button, Unit>(matchStartBtn, MatchingEvent.StartMatching, (evt, _) =>
        {
            Debug.Log("Presenter 이벤트 호출됨");
            presenter.TriggerEvent(evt);
        });
    }

    /// <summary>
    /// 초기 데이터(model)로 View 업데이트
    /// </summary>
    /// <param name="model"></param>
    public override void UpdateView(MatchingPopupUIModel model)
    {
        matchStartBtn.interactable = model.MatchBtnInteract;
    }

    public override void UpdateView<TDto>(TDto dto)
    {
        switch (dto)
        {
            case bool canStartMatch:
                Debug.Log($"실제 View 버튼에 활성화 여부 적용: {canStartMatch}");
                matchStartBtn.interactable = canStartMatch;
                break;

            default:
                Debug.LogWarning($"[UpdateView] 처리되지 않은 타입: {typeof(TDto)}");
                break;
        }
    }

}

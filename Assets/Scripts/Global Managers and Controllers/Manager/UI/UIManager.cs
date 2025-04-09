using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : IManager
{
    private Transform commonPopupContainer;

    private readonly Dictionary<string, IPopupPresenter> _findPresenters = new Dictionary<string, IPopupPresenter>();
    private readonly Stack<IPopupPresenter> _popupStack = new Stack<IPopupPresenter>();

    public void Initialize()
    {
        commonPopupContainer = GameObject.Find("UI")?.transform.Find("Popup")?.transform;
    }

    public void Release()
    {
        Clear();
    }

    /// <summary>
    /// 팝업을 보여주는 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <param name="type">팝업 타입</param>
    public async UniTaskVoid ShowPopup(string popupName)
    {
        if (!_findPresenters.TryGetValue(popupName, out var popup))
        {
            popup = await CreatePopup(popupName);

            if (!_findPresenters.ContainsKey(popupName))
            {
                _findPresenters.Add(popupName, popup);
            }
        }

        Debug.Log(popup);

        if (popup == null)
        {
            return;
        }

        popup.ShowView();

        var popupTransform = ((MonoBehaviour)popup).transform;
        popupTransform.SetAsLastSibling();

        _popupStack.Push(popup);
    }

    /// <summary>
    /// 가장 최근의 팝업을 숨기는 메서드
    /// </summary>
    public void HidePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        IPopupPresenter popup = _popupStack.Pop();
        popup.HideView();
    }

    /// <summary>
    /// 특정 팝업을 숨기는 메서드
    /// </summary>
    /// <param name="popup">팝업 프레젠터</param>
    public void HidePopup(IPopupPresenter popup)
    {
        if (_popupStack.Count <= 0)
            return;

        IPopupPresenter findPopup = _popupStack.FirstOrDefault(x => x == popup);

        if (null == findPopup)
        {
            return;
        }

        findPopup.HideView();
        _popupStack.Pop();
    }

    /// <summary>
    /// 현재 스택에 특정 이름의 팝업이 보여지고 있는지 확인하는 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <returns>존재하면 true, 아니면 false</returns>
    public IPopupPresenter IsPopupStack(string popupName)
    {
        return _popupStack.FirstOrDefault(popup => ((MonoBehaviour)popup).name.StartsWith(popupName));
    }


    /// <summary>
    /// 팝업을 생성하는 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <param name="type">팝업 타입</param>
    /// <returns>팝업 객체</returns>
    private async UniTask<IPopupPresenter> CreatePopup(string popupName)
    {
        return await GameManager.Resource.Instantiate<IPopupPresenter>(popupName, commonPopupContainer);
    }

    /// <summary>
    /// 모든 팝업과 프레젠터를 초기화하는 메서드
    /// </summary>
    private void Clear()
    {
        _findPresenters.Clear();
        _popupStack.Clear();
    }
}

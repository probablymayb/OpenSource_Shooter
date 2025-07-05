using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 게임에 존재하는 모든 UI를 통합관리하는 UI매니저
/// 팝업 관리 컨테이너, 현재 씬에 존재하는 프레젠터들을 관리한다.
/// </summary>
public class UIManager : IManager
{
    private Transform commonPopupContainer;

    private readonly Dictionary<string, IPresenter> _findPresenters = new Dictionary<string, IPresenter>();
    private readonly Stack<IPresenter> _popupStack = new Stack<IPresenter>();

    public void Initialize()
    {
        commonPopupContainer = GameObject.Find("UI")?.transform.Find("Popup")?.transform;
    }

    public void Release()
    {
        Clear();
    }

    public void FindAllPresenter()
    {
        //기존 씬에서의 프레젠터와 팝업 스택을 초기화
        Clear();

        // 씬 내의 모든 IPresenter를 찾음
        var findPresenters = Object.FindObjectsOfType<MonoBehaviour>().OfType<IPresenter>().ToArray();

        foreach (var presenter in findPresenters)
        {
            string key = presenter.ToString().Split(' ')[0];
            _findPresenters.Add(key, presenter);
            Debug.Log($"{key.ToString()} Presenter 딕셔너리에 추가");
        }

        //씬 내의 Popup 컨테이너를 찾음
        commonPopupContainer = GameObject.Find("UI")?.transform.Find("Popup")?.transform;
    }

    /// <summary>
    /// 반환값 없이 팝업을 보여주는 메서드
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
    /// 팝업을 보여줌과 동시의 팝업의 Presenter 컴포넌트를 반환한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="popupName"></param>
    /// <returns></returns>
    public async UniTask<T> ShowPopup<T> (string popupName) where T : class, IPresenter
    {
        if(!_findPresenters.TryGetValue(popupName, out var popup))
        {
            popup = await CreatePopup(popupName);
            _findPresenters.Add(popupName, popup);
        }

        if(null == popup)
        {
            return null;
        }

        popup.ShowView();

        var popupTransform = ((MonoBehaviour)popup).transform;
        popupTransform.SetAsLastSibling();

        _popupStack.Push(popup);

        return popup as T;
    }

    /// <summary>
    /// 가장 최근의 팝업을 숨기는 메서드
    /// </summary>
    public void HidePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        IPresenter popup = _popupStack.Pop();
        popup.HideView();
    }

    /// <summary>
    /// 특정 팝업을 숨기는 메서드
    /// </summary>
    /// <param name="popup">팝업 프레젠터</param>
    public void HidePopup(IPresenter popup)
    {
        if (_popupStack.Count <= 0)
            return;

        IPresenter findPopup = _popupStack.FirstOrDefault(x => x == popup);

        if (null == findPopup)
        {
            return;
        }

        findPopup.HideView();
        _popupStack.Pop();
    }

    /// <summary>
    /// 씬 프레젠터를 반환하는 메서드
    /// </summary>
    /// <typeparam name="T">프레젠터 타입</typeparam>
    /// <param name="presenterName">프레젠터 이름</param>
    /// <returns>프레젠터 객체</returns>
    public T GetScenePresenter<T>(string presenterName) where T : class
    {
        if (_findPresenters.TryGetValue(presenterName, out var findPresenter))
            return findPresenter as T;

        Debug.LogError($"{presenterName}을 Get 하지 못함");

        return null;
    }

    /// <summary>
    /// 현재 스택에 특정 이름의 팝업이 보여지고 있는지 확인하는 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <returns>존재하면 true, 아니면 false</returns>
    public IPresenter GetPopupPresenter(string popupName)
    {
        return _popupStack.FirstOrDefault(popup => ((MonoBehaviour)popup).name.StartsWith(popupName));
    }


    /// <summary>
    /// 팝업을 생성하는 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <param name="type">팝업 타입</param>
    /// <returns>팝업 객체</returns>
    private async UniTask<IPresenter> CreatePopup(string popupName)
    {
        return await GameManager.Resource.Instantiate<IPresenter>(popupName, commonPopupContainer);
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

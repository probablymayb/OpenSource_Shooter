using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PresenterBase<TModel> : MonoBehaviour, IPresenter where TModel : ModelBase
{
    // View와의 상호작용 이벤트를 관리하는 딕셔너리
    private readonly Dictionary<Enum, List<UnityAction<object>>> _events = new Dictionary<Enum, List<UnityAction<object>>>();

    [SerializeField] protected TModel model;
    [SerializeField] protected ViewBase<TModel> view;

    void Start()
    {
        model = GetComponent<TModel>();
        view = GetComponent<ViewBase<TModel>>();

        Initialize();
    }

    protected virtual void Initialize()
    {
        model.Initialize();
        view.Initialize(this);

        model.AddListener(HandleModelUpdate);

        model.TriggerEvent();
    }

    protected virtual void OnDestroy()
    {
        model?.ClearListener();
        RemoveAllListeners();
    }

    protected virtual void OnDisable()
    {
        // 일시적으로 꺼질 때도 리스너 제거하여 메모리/이벤트 누수 방지
        model?.ClearListener();
        RemoveAllListeners();
    }

    /// <summary>
    /// DTO 기반 리스너 등록
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    protected void AddListener<TDto>(Enum eventName, UnityAction<TDto> callback)
    {
        if (!_events.TryGetValue(eventName, out var actionList))
        {
            Debug.Log($"{eventName} 이벤트가 존재하지 않습니다. 새로 등록합니다.");
            actionList = new List<UnityAction<object>>();
            _events[eventName] = actionList;
        }

        actionList.Add(obj =>
        {
            Debug.Log($"이벤트 {eventName} 트리거됨! 전달된 obj: {obj}, 타입: {obj?.GetType()}");

            if (obj is TDto typed)
            {
                Debug.Log($"==> 리스너 실행됨! TDto: {typeof(TDto)}, 값: {typed}");
                callback(typed);
            }
            else
            {
                Debug.LogError($"[Presenter] 이벤트 {eventName} 잘못된 타입 전달됨! 예상: {typeof(TDto)}, 실제: {obj?.GetType()}");
            }
        });
    }

    public void TriggerEvent(Enum eventName)
    {
        TriggerEvent(eventName, Unit.Default);
    }

    public void TriggerEvent<TDto>(Enum eventName, TDto dto)
    {
        if(_events.TryGetValue(eventName,out var list))
        {
            foreach(var action in list)
            {
                action.Invoke(dto);
            }
        }
        else
        {
            Debug.LogWarning($"[Presenter] TriggerEvent: 이벤트 {eventName} 없음");
        }
    }

    /// <summary>
    /// View의 ShowView 메서드를 호출하는 메서드
    /// </summary>
    public virtual void ShowView()
    {
        if (view == null)
        {
            return;
        }

        view.ShowView();
    }

    /// <summary>
    /// View를 HideView 메서드를 호출하는 메서드
    /// </summary>
    public virtual void HideView()
    {
        if (view == null)
        {
            return;
        }

        view.HideView();
    }

    protected void RemoveAllListeners()
    {
        _events.Clear();
    }

    protected virtual void HandleModelUpdate()
    {
        view.UpdateView(model);
    }
}

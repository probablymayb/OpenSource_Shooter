using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 데이터를 보관하고 상태를 관리하는 Model의 Base클래스
/// 필요한 데이터들의 초기값 설정이 필수적이다.
/// </summary>
public abstract class ModelBase : MonoBehaviour
{
    private readonly UnityEvent _onModelChanged = new UnityEvent();

    private readonly Dictionary<Enum, Action<object>> _onModelChangedDTO = new Dictionary<Enum, Action<object>>();

    /// <summary>
    /// Model의 초기화를 진행하는 메서드
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// 모델 내 이벤트에 리스너를 추가하는 메서드
    /// </summary>
    /// <param name="listener">추가 할 리스너</param>
    public void AddListener(UnityAction listener)
    {
        _onModelChanged.AddListener(listener);
    }

    /// <summary>
    /// DTO 기반 특정 데이터 변경 이벤트만 리스너 추가하는 메서드
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public void AddListener<TDto>(Enum eventName, UnityAction<TDto> listener)
    {
        _onModelChangedDTO[eventName] = obj =>
        {
            if (obj is TDto typed)
                listener(typed);
        };
    }


    /// <summary>
    /// 모델 내 등록 된 이벤트 리스너를 제거하는 메서드
    /// </summary>
    /// <param name="listener">제거 할 리스너</param>
    public void RemoveListener(UnityAction listener)
    {
        _onModelChanged.RemoveListener(listener);
    }

    /// <summary>
    /// DTO 리스너를 개별적으로 해제 할 수 있다.
    /// </summary>
    /// <param name="eventName"></param>
    public void RemoveListener(Enum eventName)
    {
        if (_onModelChangedDTO.ContainsKey(eventName))
        {
            _onModelChangedDTO.Remove(eventName);
        }
    }

    /// <summary>
    /// 모델 내 등록 된 모든 리스너를 제거하는 메서드
    /// </summary>
    public void ClearListener()
    {
        _onModelChanged.RemoveAllListeners();
        _onModelChangedDTO.Clear();
    }

    /// <summary>
    /// 모델 내 등록 된 이벤트를 발생 시키는 메서드
    /// </summary>
    public void TriggerEvent()
    {
        _onModelChanged?.Invoke();
    }

    /// <summary>
    /// DTO 기반 이벤트를 발생 시키는 메서드
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <param name="eventName"></param>
    /// <param name="dto"></param>
    public void TriggerEvent<TDto>(Enum eventName, TDto dto)
    {
        if (_onModelChangedDTO.TryGetValue(eventName, out var action))
        {
            action.Invoke(dto);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ViewBase<TModel> : MonoBehaviour where TModel : ModelBase
{
    private UnityAction<Enum> _onTriggerAction;

    /// <summary>
    /// View의 초기화를 진행하는 메서드
    /// </summary>
    public virtual void Initialize(PresenterBase<TModel> presenter)
    {
        _onTriggerAction += presenter.TriggerEvent;
    }

    /// <summary>
    /// View를 활성화 시키는 메서드
    /// </summary>
    public virtual void ShowView()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// View를 비활성화 시키는 메서드
    /// </summary>
    public virtual void HideView()
    {
        gameObject.SetActive(false);
    }

    protected void AddUIListener<T, TDto>(T uiElement, Enum eventType, UnityAction<Enum, TDto> callback)
    where T : UIBehaviour, IEventSystemHandler
    {
        AddUIListener(uiElement, (TDto val) => callback?.Invoke(eventType, val));
    }

    private void AddUIListener<T, TDto>(
    T uiElement,
    UnityAction<TDto> listener)
    where T : UIBehaviour, IEventSystemHandler
    {
        switch (uiElement)
        {
            case Button button when typeof(TDto) == typeof(Unit):
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => listener?.Invoke(default));
                break;

            case Slider slider when typeof(TDto) == typeof(float):
                slider.onValueChanged.RemoveAllListeners();
                slider.onValueChanged.AddListener((UnityAction<float>)(object)listener);
                break;

            case Toggle toggle when typeof(TDto) == typeof(bool):
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((UnityAction<bool>)(object)listener);
                break;

            case Dropdown dropdown when typeof(TDto) == typeof(int):
                dropdown.onValueChanged.RemoveAllListeners();
                dropdown.onValueChanged.AddListener((UnityAction<int>)(object)listener);
                break;

            default:
                Debug.LogWarning($"[AddUIListener] Unsupported UI type: {typeof(T)}");
                break;
        }
    }

    public abstract void UpdateView(TModel model);

    public abstract void UpdateView<TDto>(TDto dto);
}

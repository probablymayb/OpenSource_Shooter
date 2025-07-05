using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Unit
{
    public static readonly Unit Default = new Unit();
}

public interface IPresenter
{
    void ShowView();
    void HideView();
    void TriggerEvent(Enum eventName);
    void TriggerEvent<T>(Enum eventName, T param);
}

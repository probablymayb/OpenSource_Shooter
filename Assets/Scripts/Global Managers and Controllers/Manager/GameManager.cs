using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static UIManager UI { get; } = new UIManager();
    public static SceneManager Scene { get; } = new SceneManager();
    public static ResourceManager Resource { get; } = new ResourceManager();
    public static PhotonManager Photon { get; } = new PhotonManager();

    private bool _isDestroyManager;

    public void OnApplicationQuit()
    {
        DestroyManager();
    }

    public static void Initialize()
    {
        var managers = new List<IManager>
        {
            UI,
            Scene,
            Resource,
            Photon,
        };

        foreach(var manager in managers)
        {
            manager.Initialize();
        }
    }

    private static void Relese()
    {
        Scene?.Release();
        UI?.Release();
        Resource?.Release();
        Photon?.Release();
    }

    private void DestroyManager()
    {
        if (_isDestroyManager) return;

        _isDestroyManager = true;
        Relese();
    }
}

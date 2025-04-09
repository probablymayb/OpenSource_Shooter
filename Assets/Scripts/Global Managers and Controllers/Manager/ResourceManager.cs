using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : IManager
{
    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _instantiateResource = new Dictionary<string, AsyncOperationHandle<GameObject>>();

    public void Initialize()
    {

    }

    public void Release()
    {
        _instantiateResource.Clear();
    }

    public async UniTask<GameObject> Instantiate(string address, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(address, parent);
        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = handle.Result;

            int index = obj.name.IndexOf("(Clone)", StringComparison.Ordinal);
            if (index > 0)
                obj.name = obj.name.Substring(0, index);

            _instantiateResource[obj.name] = handle;

            return obj;
        }

        Debug.Log($"Failed to instantiate resource. Address: {address}");
        return null;
    }
    public async UniTask<T> Instantiate<T>(string address, Transform parent = null) where T : class
    {
        GameObject obj = await Instantiate(address, parent);
        return obj.GetComponent<T>();
    }
}

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public static class AddressablesManager
{
    private class AssetHandle
    {
        public AsyncOperationHandle Handle;
        public int RefCount;

        public AssetHandle(AsyncOperationHandle handle)
        {
            Handle = handle;
            RefCount = 1;
        }
    }

    private static readonly Dictionary<string, AssetHandle> loadedAssets = new();

    public static async UniTask<T> LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(address))
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError("\"Address\" is Null");
#endif
            return default;
        }

        if (!loadedAssets.TryGetValue(address, out AssetHandle asset))
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            try
            {
                await handle.ToUniTask();
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogError($"<color=007FFF>[Addressables] Load Failed : {address}</color>");
#endif
                    if (handle.IsValid())
                        Addressables.Release(handle);

                    return default;
                }
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError($"<color=#007FFF>[Addressables] Load Failed : {address}</color>");
                UnityEngine.Debug.LogException(e);
#endif
                return default;
            }
            loadedAssets[address] = new AssetHandle(handle);
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"<color=#007FFF>[Addressables] Load {address} : RefCount [{loadedAssets[address].RefCount}(Load New)]</color>");
#endif
            return handle.Result;
        }

        asset.RefCount++;
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"<color=#007FFF>[Addressables] Load {address} : RefCount [{asset.RefCount - 1} -> {asset.RefCount}]</color>");
#endif
        return (T)asset.Handle.Result;
    }

    public static async UniTask<T> LoadPrefabAsync<T>(string address) where T : Component
    {
        GameObject prefab = await LoadAsync<GameObject>(address);

        if (prefab == null)
            return null;

        return prefab.GetComponent<T>();
    }


    public static void Release(string address)
    {
        if (!loadedAssets.TryGetValue(address, out AssetHandle asset))
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning($"[Addressables] Release Failed : {address}");
#endif
            return;
        }

        asset.RefCount--;
        if (asset.RefCount > 0)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log($"<color=#007FFF>[Addressables] Release {address} : RefCount [{asset.RefCount + 1} -> {asset.RefCount}]</color>");
#endif
            return;
        }

#if UNITY_EDITOR
        UnityEngine.Debug.Log($"<color=#007FFF>[Addressables] Release {address} : RefCount [{asset.RefCount}(Release Apply)]</color>");
#endif
        Addressables.Release(asset.Handle);
        loadedAssets.Remove(address);
    }

    public static void ReleaseAll()
    {
        foreach (AssetHandle asset in loadedAssets.Values)
        {
            if (asset.Handle.IsValid())
                Addressables.Release(asset.Handle);
        }

        loadedAssets.Clear();
    }
}

public static class StageAddress
{
    public static string Get(int stageId) 
        => $"StageTheme_{stageId:00}";
}

public static class StagePassageAddress
{
    public static string Get(int beforeId, int afterId) 
        => $"StagePassageTheme_{beforeId:00}_{afterId:00}";
}

public static class PlayerAddress
{
    public static string Get(int playerId) 
        => $"PlayerTheme_{playerId:00}";
}

public static class EnemyAddress
{
    public static string Get(EnemyType enemyType, int enemyId)
        => $"{enemyType.ToString()}Enemy_{enemyId:000}";
    
}
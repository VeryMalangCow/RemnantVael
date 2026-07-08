using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            UnityEngine.Debug.LogError("\"Address\" is Null");
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
                    UnityEngine.Debug.LogError($"[Addressables] Load Failed : {address}");

                    if (handle.IsValid())
                        Addressables.Release(handle);

                    return default;
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[Addressables] Load Failed : {address}");
                UnityEngine.Debug.LogException(e);

                return default;
            }
            loadedAssets[address] = new AssetHandle(handle);
            return handle.Result;
        }

        asset.RefCount++;
        return (T)asset.Handle.Result;
    }

    public static void Release(string address)
    {
        if (!loadedAssets.TryGetValue(address, out AssetHandle asset))
        {
            UnityEngine.Debug.LogWarning($"[Addressables] Release Failed : {address}");
            return;
        }

        asset.RefCount--;

        if (asset.RefCount > 0)
            return;

        Addressables.Release(asset.Handle);
        loadedAssets.Remove(address);
    }
}

public static class StageAddress
{
    public static string Get(int stageId)
    {
        return $"StageTheme_{stageId:00}";
    }
}
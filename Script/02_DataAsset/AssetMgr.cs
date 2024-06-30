using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetMgr
{
    private static Dictionary<int,    AsyncOperationHandle> mObjectHandlers = new Dictionary<int,    AsyncOperationHandle>();
    private static Dictionary<string, AsyncOperationHandle> mAssetHandler   = new Dictionary<string, AsyncOperationHandle>();

    /* Load Asset */
    public static async Task<GameObject> InstantiateGameObjectAsync(string address, Transform parent, bool isOn)
    {
        try
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, parent);
            GameObject go = await handle.Task;
            go.SetActive(isOn);

            mObjectHandlers.Add(go.GetInstanceID(), handle);
            return go;
        }
        catch
        {
            return null;
        }
    }
    private static Task<T> LoadAssetAsync<T>(string address)
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        mAssetHandler.Add(address, handle);
        return handle.Task;
    }
    public static async Task<T> SpawnUnit<T>(int code, Transform parent) where T : UnitBase, new()
    {
        string address = GetAssetAddress(EAssetType.Prefab, (int)EPrefabType.UnitBase);
        GameObject obj = await InstantiateGameObjectAsync(address, parent, true);

        T unit = new();
        unit.Awake(code, obj.transform);

        address = GetAssetAddress(EAssetType.AnimCtrl, code);
        UnityEngine.Assertions.Assert.IsNotNull(address, "Can`t Find Asset Address: " + address);
        Task<RuntimeAnimatorController> taskController = LoadAssetAsync<RuntimeAnimatorController>(address);
        await taskController;

        UnityEngine.Assertions.Assert.IsNotNull(taskController.Result, "Can`t Find Asset Data: " + address);
        unit.SetAnimeController(taskController.Result);

        taskController.Dispose();
        return unit;
    }

    /* Get Address */
    public static string GetAssetAddress(EAssetType type, int code)
    {
        int index = (byte)type * 10000 + code;

        return index switch
        {
            // Unit
            01_0001 => "AnimCtrl_Ataho",
            01_0002 => "AnimCtrl_Linxhang",
            01_0003 => "AnimeCtrl_Smashu",

            // Content
            02_0001 => "UnitBase",
            02_0002 => "OpeningGame",

            // UI
            03_0000 => "UITitle",

            // BATTLE MAP
            04_0000 => "Battle000",

            // Default
            _ => null,
        };;
    }

    /* Release Asset */
    public static bool ReleaseGameObject(int instanceID)
    {
        Addressables.Release(mObjectHandlers[instanceID]);
        return mObjectHandlers.Remove(instanceID);
    }
    public static bool ReleaseAsset(string code)
    {
        Addressables.Release(mAssetHandler[code]);
        return mAssetHandler.Remove(code);
    }
}

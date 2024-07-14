using System.Threading.Tasks;
using UnityEngine;

public partial class OnOpening // Task
{
    public class TaskOpeningInit : ITaskUpdater
    {
        public async Task<bool> Await()
        {
            string code       = AssetMgr.GetAssetAddress(EAssetType.Prefab, (int)EPrefabType.OpeningGame);
            GameObject go     = await AssetMgr.InstantiateGameObjectAsync(code, Main.UIMgr.CanvasCamera.transform, true);
            OnOpening opening = new OnOpening(go.transform);
            opening.Set();

            return true;
        }
    }
}


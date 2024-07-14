using System.Threading.Tasks;
using UnityEngine;

public partial class UIMgr // Task
{
    public class TaskUILoad : ITaskUpdater
    {
        private EGameStateFlag mState;
        public TaskUILoad(EGameStateFlag state)
        {
            mState = state;
        }
        public async Task<bool> Await()
        {
            switch (mState)
            {
                case EGameStateFlag.Opening:
                    UIMgr uiMgr = Main.UIMgr;
                    string code = AssetMgr.GetAssetAddress(EAssetType.UI, (int)EUIType.Title);
                    GameObject obj = await AssetMgr.InstantiateGameObjectAsync(code, uiMgr.CanvasCamera.transform, false);
                    UIOpening title = obj.AddComponent<UIOpening>();
                    uiMgr.mUICache[(byte)EUIType.Title] = title;
                    break;
                case EGameStateFlag.EnterGame:
                    Debug.Log("Need to dev: UI.InitAsync(Field)");
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}

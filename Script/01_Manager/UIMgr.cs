using System.Threading.Tasks;
using UnityEngine;

public partial class UIMgr
{
    private UIBase[] mUICache;
    public Canvas CanvasOverlay { get; private set; }
    public Canvas CanvasCamera  { get; private set; }

    public async Task InitAsync(EGameStateFlag state)
    {
        switch (state)
        {
            case EGameStateFlag.Opening:
                string code = AssetMgr.GetAssetAddress(EAssetType.UI, (int)EUIType.Title);
                GameObject obj = await AssetMgr.InstantiateGameObjectAsync(code, CanvasCamera.transform, false);
                UIOpening title = obj.AddComponent<UIOpening>();
                mUICache[(byte)EUIType.Title] = title;
                break;
            case EGameStateFlag.EnterGame:
                Debug.Log("Need to dev: UI.InitAsync(Field)");
                break;
        }
    }
    public void Pop(EUIType type, bool isOn)
    {
        mUICache[(byte)type].Pop(isOn);
    }
    public void Release()
    {
        UnityEngine.Assertions.Assert.IsNotNull(mUICache, "null ui cache");

        for (int i = 0; i < mUICache.Length; ++i)
        {
            if (null == mUICache[i])
            {
                break;
            }

            mUICache[i].Release();
        }
    }

    public UIMgr(Transform transform)
    {
        mUICache = new UIBase[8]; //���Ƿ� ����
        CanvasOverlay = transform.GetChild(0).GetComponent<Canvas>();
        CanvasCamera  = transform.GetChild(1).GetComponent<Canvas>();
    }
}

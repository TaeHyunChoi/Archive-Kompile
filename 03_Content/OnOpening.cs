using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class OnOpening
{
    private static OnOpening instance;
    private Transform transform;
    private int mState;

    public static async Task<OnOpening> InitAsync(Transform canvas_camera)
    {
        string code = AssetMgr.GetAssetAddress(EAssetType.Prefab, (int)EPrefabType.OpeningGame);
        GameObject go = await AssetMgr.InstantiateGameObjectAsync(code, canvas_camera, true);
        OnOpening opening = new OnOpening(go.transform);
        return opening;
    }

    public void Set()
    {
        switch (mState)
        {
            case 0:
                IEOpeningLogo logo = new IEOpeningLogo(transform.GetChild(0));
                CoroutineUpdater.SetHandler(new CCoroutine<IEOpeningLogo>(logo));
                break;
            case 1:
                IEOpeningDemo demo = new IEOpeningDemo(transform.GetChild(1));
                CoroutineUpdater.SetHandler(new CCoroutine<IEOpeningDemo>(demo));
                break;
            case 2:
                IEOpeningTitle title = new IEOpeningTitle(transform.GetChild(2));
                CoroutineUpdater.SetHandler(new CCoroutine<IEOpeningTitle>(title));
                break;
            case 3:
                Main.UIMgr.Pop(EUIType.Title, true);
                break;
            default:
                return;
        }

        mState += 1;
    }
    public static void Release()
    {
        GameObject obj = instance.transform.gameObject;
        GameObject.Destroy(obj);
        if (false == AssetMgr.ReleaseGameObject(obj.GetInstanceID()))
        {
            Debug.LogError($"Can`t Release Asset: {obj.name}({obj.GetInstanceID()})");
        }
    }

    private OnOpening(Transform transform)
    {
        instance = this;
        this.transform = transform;
        mState = 0;

        Image[] img = transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < img.Length; ++i)
        {
            img[i].color = new Color(1f, 1f, 1f, 0f);
        }
    }
}

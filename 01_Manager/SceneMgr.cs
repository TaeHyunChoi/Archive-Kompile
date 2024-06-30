using UnityEngine;
using DataStruct;

public partial class SceneMgr
{
    private static CanvasGroup mCurtainCanvas;

    public void LoadSceneAsync(EGameStateFlag next, int code = -1)
    {
        Main.ClearInput();

        switch (next)
        {
            case EGameStateFlag.Opening:
                IEInitOpeningScene opening = new IEInitOpeningScene(mCurtainCanvas);
                CoroutineUpdater.SetHandler(new CCoroutine<IEInitOpeningScene>(opening));
                break;
            case EGameStateFlag.EnterGame:
                //TODO: Load(or Get) Play data. => chaper: 
                IEEnterIngame level = new IEEnterIngame(chapter: 0);
                CoroutineUpdater.SetHandler(new CCoroutine<IEEnterIngame>(level));
                break;
        }
    }

    public SceneMgr(Transform transformCanvas)
    {
        transformCanvas = transformCanvas.GetChild(0);
        mCurtainCanvas = transformCanvas.GetComponent<CanvasGroup>();
        mCurtainCanvas.gameObject.SetActive(false);
    }
}

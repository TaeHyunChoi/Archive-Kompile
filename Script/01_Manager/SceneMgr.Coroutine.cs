using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneMgr;

public partial class SceneMgr // Coroutine
{
    public class IEInitOpeningScene : IRoutineUpdater
    {
        private AsyncOperation  mLoadAsyncOper;
        private CanvasGroup     mCurtainCanvas;
        private Task<OnOpening> mTaskOpening;
        private Task            mTaskLoadUI;

        public int MoveNext(int index)
        {
            switch (index)
            {
                case 0:
                    mCurtainCanvas.gameObject.SetActive(true);
                    break;
                case 1:
                    mLoadAsyncOper = SceneManager.LoadSceneAsync("010_OpeningScene", LoadSceneMode.Single);
                    break;
                case 2:
                    if (false == mLoadAsyncOper.isDone)
                    {
                        return index;
                    }
                    break;
                case 3:
                    Transform transformCameraCanvas = Main.UIMgr.CanvasCamera.transform;
                    mTaskOpening = OnOpening.InitAsync(transformCameraCanvas);
                    mTaskLoadUI = Main.UIMgr.InitAsync(EGameStateFlag.Opening);
                    break;
                case 4:
                    if (false == mTaskOpening.IsCompletedSuccessfully
                        || false == mTaskLoadUI.IsCompletedSuccessfully)
                    {
                        return index;
                    }

                    mTaskOpening.Result.Set();
                    mCurtainCanvas.gameObject.SetActive(false);
                    break;
                default:
                    mTaskOpening.Dispose();
                    mTaskLoadUI.Dispose();
                    return -1;
            }

            return index + 1;
        }

        public IEInitOpeningScene(CanvasGroup curtain)
        {
            mCurtainCanvas = curtain;
        }
    }
    public class IEEnterIngame : IRoutineUpdater
    {
        private AsyncOperation  mLoadAsyncOper;
        private Task<FieldContent>   mTaskField;
        private Task<BattleContent>  mTaskBattle;
        private Task            mTaskLoadUI;

        //private MapData         mMapData;
        private int mChapter;

        private IECurtainOn  mCurtainOn;
        private IECurtainOff mCurtainOff;

        public int MoveNext(int index)
        {
            switch (index)
            {
                case 0:
                    CoroutineUpdater.SetHandler(new CCoroutine<IECurtainOn>(mCurtainOn));
                    break;
                case 1:
                    if (false == mCurtainOn.IsDone)
                    {
                        return index;
                    }
                    mCurtainOn = null;
                    break;
                case 2:
                    //TODO: dev Mapdata (using grid?)
                    string sceneName = string.Empty;
                    switch (mChapter)
                    {
                        case 0: sceneName = "000_FieldTestScene"; break;
                        default:
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
                            Debug.LogError($"Fail to load scene: Wrong chapter({mChapter})");
#endif
                            return -1;
                    }

                    mLoadAsyncOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                    OnOpening.Release();
                    Main.Release();
                    break;
                case 3:
                    if (false == mLoadAsyncOper.isDone)
                    {
                        return index;
                    }
                    mLoadAsyncOper = null;
                    break;
                case 4:
                    mTaskField  = FieldContent.InitAsync();
                    mTaskBattle = BattleContent.InitAsync(mChapter);
                    mTaskLoadUI = Main.UIMgr.InitAsync(EGameStateFlag.EnterGame);
                    break;
                case 5:
                    if (false == mTaskField.IsCompletedSuccessfully
                        || false == mTaskBattle.IsCompletedSuccessfully
                        || false == mTaskLoadUI.IsCompletedSuccessfully)
                    {
                        return index;
                    }

                    Main.SetContenData(EContentType.Field,  mTaskField.Result);
                    Main.SetContenData(EContentType.Battle, mTaskBattle.Result);
                    Main.SetCurrentContent(EContentType.Field);
                    break;
                case 6:
                    CoroutineUpdater.SetHandler(new CCoroutine<IECurtainOff>(mCurtainOff));
                    break;
                case 7:
                    if (false == mCurtainOff.IsDone)
                    {
                        return index;
                    }
                    mCurtainOff = null;
                    break;
                default:
                    mTaskField.Dispose();
                    mTaskBattle.Dispose();
                    mTaskLoadUI.Dispose();
                    Main.GetContent<FieldContent>().Start();
                    return -1;
            }

            return index + 1;
        }

        public IEEnterIngame(int chapter)
        {
            mChapter    = chapter;
            mCurtainOn  = new IECurtainOn();
            mCurtainOff = new IECurtainOff();
        }
    }

    public class IECurtainOn : IRoutineUpdater
    {
        public bool IsDone { get; set; }

        private CanvasGroup mCurtainCanvas;
        private float mFadeSpeedMultiple;

        public int MoveNext(int index)
        {
            switch (index)
            {
                case 0:
                    IsDone = false;
                    mCurtainCanvas.alpha = 0;
                    mCurtainCanvas.gameObject.SetActive(true);
                    break;
                case 1:
                    if (mCurtainCanvas.alpha < 1)
                    {
                        mCurtainCanvas.alpha += Time.fixedDeltaTime * mFadeSpeedMultiple;
                        return index;
                    }
                    break;
                default:
                    mCurtainCanvas.alpha = 1;
                    IsDone = true;
                    return -1;
            }

            return index + 1;
        }
        public IECurtainOn(float speedMultiple = 1f)
        {
            mCurtainCanvas = SceneMgr.mCurtainCanvas;
            mFadeSpeedMultiple = speedMultiple;
        }
    }
    public class IECurtainOff : IRoutineUpdater
    {
        private CanvasGroup mCurtainCanvas;
        public bool IsDone { get; set; }

        public int MoveNext(int index)
        {
            switch (index)
            {
                case 0:
                    if (mCurtainCanvas.alpha > 0)
                    {
                        mCurtainCanvas.alpha -= Time.fixedDeltaTime * 3;
                        return index;
                    }
                    break;
                default:
                    mCurtainCanvas.alpha = 0;
                    mCurtainCanvas.gameObject.SetActive(false);
                    IsDone = true;
                    return -1;

            }
            return index + 1;
        }
        public IECurtainOff()
        {
            mCurtainCanvas = SceneMgr.mCurtainCanvas;
            IsDone = false;
        }
    }

    //later...
    public class LoadFieldScene : IRoutineUpdater
    {
        public int MoveNext(int index)
        {

            return -1;
        }
    }
}

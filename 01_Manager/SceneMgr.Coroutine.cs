using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneMgr // Coroutine
{
    public class IEInitOpeningScene : IRoutineUpdater
    {
        private AsyncOperation  mLoadAsyncOper;
        private CanvasGroup     mCurtainCanvas;


        private CTaskHandler<OnOpening.TaskOpeningInit> mTaskOpeningInit;
        private CTaskHandler<UIMgr.TaskUILoad>          mTaskUILoad;
        
        public IEInitOpeningScene(CanvasGroup curtain)
        {
            mCurtainCanvas = curtain;
        }
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
                    var taskOpeningInit = new OnOpening.TaskOpeningInit();
                    mTaskOpeningInit = new CTaskHandler<OnOpening.TaskOpeningInit>(taskOpeningInit);

                    var taskUI = new UIMgr.TaskUILoad(EGameStateFlag.Opening);
                    mTaskUILoad = new CTaskHandler<UIMgr.TaskUILoad>(taskUI);
                    break;
                case 4:
                    bool isDone = mTaskOpeningInit.IsDone
                                  & mTaskUILoad.IsDone;

                    if (false == isDone)
                    {
                        return index;
                    }

                    mCurtainCanvas.gameObject.SetActive(false);
                    mTaskOpeningInit.Dispose();
                    mTaskUILoad.Dispose();
                    break;
                default:
                    return -1;
            }

            return index + 1;
        }
    }
    public class IEEnterIngame : IRoutineUpdater
    {
        private IECurtainOn  mCurtainOn;
        private IECurtainOff mCurtainOff;

        private AsyncOperation                             mLoadAsyncOper;
        private CTaskHandler<FieldContent.TaskFieldInit>   mTaskInitField;
        private CTaskHandler<BattleContent.TaskBattleInit> mTaskInitBattle;
        private CTaskHandler<UIMgr.TaskUILoad>             mTaskInitUI;

        private int mChapter;

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
                    break;
                case 4:
                    var taskField = new FieldContent.TaskFieldInit();
                    mTaskInitField = new CTaskHandler<FieldContent.TaskFieldInit>(taskField);

                    var taskBattle = new BattleContent.TaskBattleInit(mChapter);
                    mTaskInitBattle = new CTaskHandler<BattleContent.TaskBattleInit>(taskBattle);

                    var taskUI = new UIMgr.TaskUILoad(EGameStateFlag.EnterGame);
                    mTaskInitUI = new CTaskHandler<UIMgr.TaskUILoad>(taskUI);

                    break;
                case 5:
                    bool isDone = mTaskInitField.IsDone 
                                  & mTaskInitBattle.IsDone
                                  & mTaskInitUI.IsDone;

                    if (false == isDone)
                    {
                        return index;
                    }
                    break;
                case 6:
                    CoroutineUpdater.SetHandler(new CCoroutine<IECurtainOff>(mCurtainOff));
                    break;
                case 7:
                    if (false == mCurtainOff.IsDone)
                    {
                        return index;
                    }

                    //sync
                    mCurtainOn = null;
                    mCurtainOff = null;

                    //async
                    mLoadAsyncOper = null;
                    mTaskInitField.Dispose();
                    mTaskInitBattle.Dispose();
                    mTaskInitUI.Dispose();

                    GC.Collect();

                    break;
                default:
                    Main.SetCurrentContent(EContentType.Field);
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

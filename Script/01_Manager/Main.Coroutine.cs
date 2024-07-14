using UnityEngine;
using static SceneMgr;

public partial class Main // Coroutine
{
    public class IEBattleEnter : IRoutineUpdater
    {
        private IECurtainOn  mCurtainOn;
        private IECurtainOff mCurtainOff;

        private Vector3 mMapPosition;
        private float   mTimeWait = 0.10f;
        private int     mMapCode;

        public IEBattleEnter(int mapCode, Vector3 mapPosition)
        {
            mMapCode = mapCode;
            mMapPosition = mapPosition;

            mCurtainOn  = new IECurtainOn(speedMultiple: 2f);
            mCurtainOff = new IECurtainOff();
        }
        public int MoveNext(int index)
        {
            switch (index)
            {
                case 0:
                    InputMgr.Clear();
                    mTimeWait = 0.10f;
                    mCurrentContent = null;
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
                    mFieldContent.SetLayer(-1);
                    mBattleContent.Set(mMapCode, mMapPosition);
                    mTimeWait += Time.time;
                    break;
                case 3:
                    if (Time.time < mTimeWait)
                    {
                        return index;
                    }
                    break;
                case 4:
                    CoroutineUpdater.SetHandler(new CCoroutine<IECurtainOff>(mCurtainOff));
                    break;
                case 5:
                    if (false == mCurtainOff.IsDone)
                    {
                        return index;
                    }

                    mCurtainOn  = null;
                    mCurtainOff = null;
                    break;
                default:
                    return -1;
            }

            return index + 1;
        }
    }
}

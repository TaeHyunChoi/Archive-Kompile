public class CCoroutineHandler
{
    protected int mindex;
    public virtual bool MoveNext()
    {
        return false;
    }
}
public class CCoroutine<T> : CCoroutineHandler where T : class, IRoutineUpdater
{
    private T mRoutine;

    public CCoroutine(T data)
    {
        this.mRoutine = data;
        mindex = 0;
    }
    public override bool MoveNext()
    {
        mindex = mRoutine.MoveNext(mindex);

        if (-1 == mindex)
        {
            mRoutine = null;
            return false;
        }

        //updater에서 = null; 하는게 아니라 '호출한 곳에서 null 처리' 하는게 맞는거구나...
        //ㅇㅋㅇㅋ 그러면 오늘 이거 수정하고 + 커튼 착착 쓰면 되겠다?

        return true;
    }
}
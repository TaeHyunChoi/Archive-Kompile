public class CCoroutineHandler
{
    protected int mIndex;
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
        mRoutine = data;
        mIndex = 0;
    }
    public override bool MoveNext()
    {
        mIndex = mRoutine.MoveNext(index);

        if (-1 == mIndex)
        {
            mRoutine = null;
            return false;
        }

        return true;
    }
}

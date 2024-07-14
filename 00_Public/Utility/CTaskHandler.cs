using System.Threading.Tasks;

public class CTaskHandler<T> where T : class, ITaskUpdater
{
    private Task<bool> mTask;

    public bool IsDone
    {
        get
        {
            if (null == mTask
                || false == mTask.IsCompleted)
            {
                return false;
            }

            return mTask.Result && mTask.IsCompletedSuccessfully;
        }
    }

    public CTaskHandler(T task)
    {
        mTask = task.Await();
    }
    public void Dispose()
    {
        if (null != mTask)
        {
            mTask.Dispose();
        }
    }
}
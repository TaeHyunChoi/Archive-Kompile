public abstract class ContentBase
{
    protected int mState = 0;
    public abstract void Release();
    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
}

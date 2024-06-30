using UnityEngine;
using static EAnimeCodeToString;

public abstract class UnitBase
{
    public Transform transform { get; set; }
    public GameObject gameObject { get; set; }

    protected AnimationClip[] mAnimationClips;
    protected Animator        mAnimator;
    protected int             mUnitCode;

    public virtual void Awake(int codeUnit, Transform transform)
    {
        this.mUnitCode  = codeUnit;
        this.transform = transform;
        this.gameObject = transform.gameObject;
        mAnimator = transform.GetComponent<Animator>();
    }

    public void SetAnimeController(RuntimeAnimatorController controller)
    {
        mAnimator.runtimeAnimatorController = controller;
        PlayAnime(IDLE_FRONT);
    }
    protected void PlayAnime(EAnimeCodeToString code)
    {
        string anime = null;
        switch (code)
        {
            default:
                anime = code.ToString();
                break;
            case NONE:
                break;
        }

        mAnimator.Play(anime, 0);
    }
    public bool Release()
    {
        //TODO: 오브젝트 풀링을 해야 할까?
        GameObject.Destroy(gameObject);

        string address = AssetMgr.GetAssetAddress(EAssetType.AnimCtrl, mUnitCode);
        return AssetMgr.ReleaseAsset(address);
    }
}
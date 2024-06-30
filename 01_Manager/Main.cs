using DataStruct;
using System.ComponentModel;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    //TODO: FrameRate는 추후에 Config.cs 등에게 넘기기
    [SerializeField]
    private int mFrameRate = 144;

    public static Main         Instance { get; private set; }

    public static InputMgr     InputMgr { get; private set; }
    public static SceneMgr     SceneMgr { get; private set; }
    public static UIMgr        UIMgr    { get; private set; }
    public static CameraFollow Cam      { get; private set; }

    private static ContentBase   mCurrentContent;
    private static FieldContent  mFieldContent;
    private static BattleContent mBattleContent;

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
    private float deltaTime = 0.0f;
#endif

    private void Awake()
    {
        //like Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        DataTable.LoadContentTable();
        //TODO: Load Player Saved Data
        PlayData.LoadData( null /*saved_data*/);

        InputMgr = transform.GetComponent<InputMgr>();
        Cam      = Camera.main.transform.GetComponent<CameraFollow>();
        UIMgr    = new UIMgr   (transform.Find("UI"));
        SceneMgr = new SceneMgr(UIMgr.CanvasOverlay.transform);
    }
    private void Start()
    {
        SceneMgr.LoadSceneAsync(EGameStateFlag.Opening);
        Application.targetFrameRate = mFrameRate;
    }

    //클래스를 명시하고자 <T>를 사용
    public static T GetContent<T>() where T : ContentBase
    {
        return mCurrentContent as T;
    }
    public static void SetContenData(EContentType type, ContentBase content)
    {
        switch (type)
        {
            case EContentType.Field:  
                mFieldContent = content as FieldContent;
                UnityEngine.Assertions.Assert.IsNotNull(mFieldContent);
                break;
            case EContentType.Battle: 
                mBattleContent = content as BattleContent;
                UnityEngine.Assertions.Assert.IsNotNull(mBattleContent);
                break;
            default:
                Debug.LogError("[SetContent] Wrong type:"); 
                return;
        }
    }
    public static void SetCurrentContent(EContentType type)
    {
        switch (type)
        {
            case EContentType.Field:  mCurrentContent = mFieldContent;  break;
            case EContentType.Battle: mCurrentContent = mBattleContent; break;
            default: Debug.LogError("[Set Current Content] Wrong type:"); return;
        }
    }

    public static void EnterBattle(int mapCode, Vector3 mapPosition)
    {
        IEBattleEnter enterBattle = new IEBattleEnter(mapCode, mapPosition);
        CoroutineUpdater.SetHandler(new CCoroutine<IEBattleEnter>(enterBattle));
    }

    public static void SetInput(IInputHandler input)
    {
        InputMgr.Updater = input;
    }
    public static void ClearInput()
    {
        InputMgr.Updater = null;
    }


    private void Update()
    {
        mCurrentContent?.Update();

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
#endif
    }
    private void FixedUpdate()
    {
        mCurrentContent?.FixedUpdate();
    }


    public static void Release()
    {
        UIMgr.Release();
    }



#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0f, 1f, 0f, 1f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
#endif
}

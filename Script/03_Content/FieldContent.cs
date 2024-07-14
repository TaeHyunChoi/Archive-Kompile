using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DataStruct;
using UnityEngine.SceneManagement;

public partial class FieldContent : ContentBase, IInputHandler
{
    private Dictionary<int, STile> mTileMap;

    private UnitPlayer player;

    //private UnitBase[3]     mPlayerParty;
    //private List<UnitBase>  mNPC;

    private MapTileComponent[] mTileComponents;
    public Transform LevelTransform { get; set; }

    private Index.IDxInput.EInput mInput;
    private int mMaxEncounterValue = 200;
    private int mEncounterValue;

    private FieldContent(Transform level)
    {
        Debug.Log("Test Map: 000_FieldTest");

        mTileMap = DataTable.LoadMappingTable<STile>("000_FieldTest");
        this.LevelTransform = level;
    }
    public override void Start()
    {
        Main.SetInput(this);
    }


    public void SetLayer(int layerOn)
    {
        //TODO: 더욱 효율적인 방법이 있을 것이다 (ex. 해당 구역만 로딩 -> ActiveParent, InactiveParent ?)
        MapTileComponent tile;
        mTileComponents = LevelTransform.GetComponentsInChildren<MapTileComponent>(true);
        for (int i = 0; i < mTileComponents.Length; ++i)
        {
            tile = mTileComponents[i];
            if (tile.Layer == layerOn)
            {
                TransMapTile trans = new TransMapTile(tile);
                CoroutineUpdater.SetHandler(new CCoroutine<TransMapTile>(trans));
            }
            else
            {
                tile.gameObject.SetActive(false);
            }
        }

        if (-1 == layerOn)
        {
            Main.Cam.StopFollow();
            player.gameObject.SetActive(false);

        }
        else
        {
            Main.Cam.SetFollow(player.transform);
        }

        mTileComponents = null;
    }


    public void Input(Index.IDxInput.EInput input)
    {
        mInput = input;
        //goto FixedUpdate();
    }
    public override void Update()
    {
        //not_yet
        //field.Update()가 필요할 수 있다.
        //ex. npc behaviour
    }
    public override void FixedUpdate()
    {
        player.Update(mTileMap, mInput);
    }
    
    
    public void Encounter(int mapCode)
    {
        mEncounterValue += 1;

        if (mMaxEncounterValue < mEncounterValue)
        {
            Debug.Log($"code[{mapCode}].encount = {mEncounterValue}/{mMaxEncounterValue}");

            mEncounterValue = 0;
            mMaxEncounterValue = Random.Range(400, 1000); //임시값

            player.StopMove();
            Main.EnterBattle(mapCode, player.transform.position);
        }
    }


    public override void Release()
    {
        // later...
        //mTileMap = null;
        //player = null;
        //Main.Field = null;
    }
}

using DataStruct;
using System.Threading.Tasks;
using UnityEngine;

public partial class BattleContent : ContentBase
{
    private static readonly byte COUNT_MAP      = 4;
    private static readonly byte COUNT_MAX_UNIT = 7;

    private Transform[]          mUnitTransformArray;
    private UnitBattle[]         mBattleUnitArray;
    private BattleMapComponent[] mBattleMapArray;
    private Transform            transform;

    private int mLengthInBattle;

    /* initialize */
    public static async Task<BattleContent> InitAsync(int chapter)
    {
        BattleContent battle = new BattleContent(transform: Main.Instance.transform.Find("Battle"));

        string code;
        GameObject map;
        for (int i = 0; i < COUNT_MAP; ++i)
        {
            code = AssetMgr.GetAssetAddress(EAssetType.BattleMap, chapter * 100 + i);
            map = await AssetMgr.InstantiateGameObjectAsync(code, battle.transform, false);

            if (null != map)
            {
                battle.mBattleMapArray[i] = map.transform.GetComponent<BattleMapComponent>();
            }
        }

        return battle;
    }

    /* set battle */
    public void Set(int mapCode, Vector3 lastPosition)
    {
        // 전투맵 설정
        if (false == DataTable.TryGetMapData(mapCode, out MapData dataMap))
        {
            Debug.LogError("null map_data: " + mapCode);
            return;
        }

        Vector3 pos = lastPosition;
        for (int i = 0; i < COUNT_MAP; ++i)
        {
            bool isActive = (mapCode == mBattleMapArray[i].Code);

            mBattleMapArray[i].SetActive(isActive);
            if (true == isActive)
            {
                pos = mBattleMapArray[i].Position;
                break;
            }
        }

        int index;

        UnitPartyData party;
        for (index = 0; index < PlayData.PartyData.Length; ++index)
        {
            party = PlayData.PartyData[index];
            mBattleUnitArray[index] = new UnitBattle(party.UnitData, party.Level, mUnitTransformArray[index]);
        }

        SelectEnemiesByRandom(dataMap, indexStart: index, mBattleUnitArray, out  mLengthInBattle);

        //TODO: battle_map 상에서의 각 포지션 설정
        //SetUnitPosition(null);

        // 현재 콘텐츠로 설정
        Main.Cam.transform.position = pos + new Vector3(0, 3f, -2f);
        Main.SetCurrentContent(EContentType.Battle);
    }

    private void SelectEnemiesByRandom(MapData mapData, int indexStart, UnitBattle[] arrayUnit, out int length)
    {
        int tierPlayer = PlayData.PartyTier;
        int sumTierMax = tierPlayer * 2;
        int sumTier = 0;

        int index = indexStart;
        int indexEnd = indexStart + Random.Range(1, mapData.MaxENMCount);

        length = index;
        int lengthMonsterType = mapData.EnemyTypeArray.Length;

        while (sumTier < sumTierMax && index < indexEnd)
        {
            int indexRandom = Random.Range(0, lengthMonsterType);

            /*case 1: 임의의 인덱스에 있는 유닛 뽑기 (enemy.tier는 뽑기 고려 기준이 아니다.)*/
            if (true == DataTable.TryGetUnitData(mapData.EnemyTypeArray[indexRandom], out UnitData enemyData))
            {
                int enemy_tier = enemyData.Tier;
                if (sumTier + enemy_tier <= sumTierMax)
                {
                    sumTier += enemy_tier;
                }
            }
            
            /*case 2: 할당 가능한 티어가 없다면 탐색 종료 (반복문 종료)*/
            else if (false == TryGetOtherEnemy(sumTierMax - sumTier, out enemyData))
            {
                break;
            }

            /*case 3: 그런 유닛 코드는 존재하지 않는다*/
            else
            {
                Debug.LogError("Wrong unit code");
                return;
            }

            byte level = (byte)(enemyData.Tier * (Random.Range(1, 4)));
            arrayUnit[index] = new UnitBattle(enemyData, level, mUnitTransformArray[index]);
            
            ++index;
        }

        length = index;
    }
    private bool TryGetOtherEnemy(int tierRemain, out UnitData enemyData)
    {
        enemyData = default(UnitData);

        return false;
    }


    public override void Start()
    {
        Main.ClearInput();
        Debug.Log($"TODO Dev: Set Battle");
    }
    public override void Release()
    {
        mBattleUnitArray = null;
    }

    //TODO: 콘텐츠 관련 설계를 변경할 예정 (만들고 나니 ContentBase가 쓸모 없다.)
    public override void Update()
    {
        //not_used
    }
    public override void FixedUpdate()
    {
        //not_used
    }


    private BattleContent(Transform transform)
    {
        this.transform = transform;

        transform = transform.GetChild(0);
        mUnitTransformArray = new Transform[COUNT_MAX_UNIT];
        for (int i = 0; i < COUNT_MAX_UNIT; ++i)
        {
            mUnitTransformArray[i] = transform.GetChild(i);
        }

        mBattleUnitArray = new UnitBattle[COUNT_MAX_UNIT];
        mBattleMapArray = new BattleMapComponent[COUNT_MAP];
    }
}

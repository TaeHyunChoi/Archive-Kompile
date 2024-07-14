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

    /* set battle */
    public void Set(int codeMap, Vector3 position)
    {
        // 전투맵 설정
        if (false == DataTable.TryGetMapData(codeMap, out MapData dataMap))
        {
            Debug.LogError("null map_data: " + codeMap);
            return;
        }
        transform.position = position;

        BattleMapComponent map;
        for (int i = 0; i < COUNT_MAP; ++i)
        {
            map = mBattleMapArray[i];
            if (null != map)
            {
                map.SetActive(isOn: codeMap == map.Code);
            }
        }

        int index;
        int lengthParty;

        // set party
        UnitPartyData party;
        lengthParty = PlayData.PartyData.Length;
        for (index = 0; index < lengthParty; ++index)
        {
            party = PlayData.PartyData[index];
            mBattleUnitArray[index] = new UnitBattle(party.UnitData, party.Level, mUnitTransformArray[index]);
        }
        party = null;

        // set enemies
        SelectEnemiesByRandom(dataMap, indexStart: index, mBattleUnitArray, out mLengthInBattle);

        // set
        SetUnits(mBattleUnitArray, lengthParty, mLengthInBattle - lengthParty);

        // 

        // 현재 콘텐츠로 설정
        Main.Cam.transform.position = position + new Vector3(0, 3f, -2f);
        Main.SetCurrentContent(EContentType.Battle);
    }
    private void SetUnits(UnitBattle[] array, int lengthParty, int lengthEnemy)
    {
        float x, z;
        float sx, sz;

        int index;
        
        index = 0;
        x = -2f;
        z = 0.6f;

        switch (lengthParty)
        {
            case 1:
                mBattleUnitArray[index].SetBattle(new Vector3(x, 0, z));
                break;
            case 2:
                sx = 0.15f;
                sz = 0.3f;
                mBattleUnitArray[index].SetBattle(new Vector3(x + sx, 0, z + sz));
                mBattleUnitArray[index + 1].SetBattle(new Vector3(x - sx, 0, z - sz));
                break;
            case 3:
                sx = 0.45f;
                sz = 0.6f;
                mBattleUnitArray[index].SetBattle(new Vector3(x + sx, 0, z + sz));
                mBattleUnitArray[index + 1].SetBattle(new Vector3(x, 0, z));
                mBattleUnitArray[index + 2].SetBattle(new Vector3(x - sz, 0, z - sz));
                break;
            default:
                {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
                    Debug.LogError("wrong party length");
#endif                
                }
                return;
        }

        index = lengthParty;
        x = 2f;
        switch (lengthEnemy)
        {
            case 1:
                mBattleUnitArray[index].SetBattle(new Vector3(x, 0, z));
                break;
            case 2:
                sx = 0.15f;
                sz = 0.25f;
                mBattleUnitArray[index].SetBattle(new Vector3(x - sx, 0, z + sz));
                mBattleUnitArray[index + 1].SetBattle(new Vector3(x + sx, 0, z - sz));
                break;
            case 3:
                sx = 0.45f;
                sz = 0.6f;
                mBattleUnitArray[index].SetBattle(new Vector3(x - sx, 0, z + sz));
                mBattleUnitArray[index + 1].SetBattle(new Vector3(x, 0, z));
                mBattleUnitArray[index + 2].SetBattle(new Vector3(x + sz, 0, z - sz));
                break;
            case 4:
                z = 0f;
                sx = 0.45f;
                sz = 0.5f;
                mBattleUnitArray[index].SetBattle(new Vector3(x - sx * 2, 0, z + sz * 2));
                mBattleUnitArray[index + 1].SetBattle(new Vector3(x - sx, 0, z + sz));
                mBattleUnitArray[index + 2].SetBattle(new Vector3(x, 0, z));
                mBattleUnitArray[index + 3].SetBattle(new Vector3(x + sx, 0, z - sz));
                break;
            default:
                {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
                    Debug.LogError("wrong party length");
#endif                
                }
                return;
        }
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
    }
    public override void Release()
    {
        mBattleUnitArray = null;
    }


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

    /* maybe later? */
    //private void SetUnitPosition(UnitBattle pawn)
    //{
    //    for (int i = 0; i < mBattleUnitArray.Length; ++i)
    //    {
    //        pawn = mBattleUnitArray[i];
    //        switch (i)
    //        {
    //            // party
    //            case 0: pawn.transform.position = pos + new Vector3(-0.900f - 1f, 0, +1.200f); break;
    //            case 1: pawn.transform.position = pos + new Vector3(-1.200f - 1f, 0, +0.400f); break;
    //            case 2: pawn.transform.position = pos + new Vector3(-1.500f - 1f, 0, -0.400f); break;

    //            //enemy
    //            case 3: pawn.transform.position = pos + new Vector3(+2.000f - 0.3f, 0, +1.500f); break;
    //            case 4: pawn.transform.position = pos + new Vector3(+2.200f - 0.3f, 0, +0.900f); break;
    //            case 5: pawn.transform.position = pos + new Vector3(+2.400f - 0.3f, 0, +0.300f); break;
    //            case 6: pawn.transform.position = pos + new Vector3(+2.600f - 0.3f, 0, -0.300f); break;
    //        }
    //        pawn.gameObject.SetActive(true);
    //    }
    //}
    //private void SelectEnemiesByRandom(MapData mapData, out UnitBattle[] enemies, out int length)
    //{
    //    length = Random.Range(1, mapData.MaxENMCount);
    //    enemies = new UnitBattle[length];

    //    //언더바 사용(_): 개념을 아직 구현하지 않았음
    //    int player_tier = PlayData.PartyTier;
    //    int map_tier_sum = 2;

    //    int tierSumMax = (map_tier_sum > player_tier) ? player_tier * 2 : map_tier_sum;
    //    int tierSum = 0;

    //    int index = 0;
    //    int map_monster_type_length = mapData.EnemyTypeArray.Length;

    //    while (tierSum < tierSumMax && index++ < length)
    //    {
    //        int indexRandom = Random.Range(0, map_monster_type_length);
    //        if (true == DataTable.TryGetUnitData(mapData.EnemyTypeArray[indexRandom], out UnitData enemyData))
    //        {
    //            //int tier = enemyData.Tier;
    //            /* tier가 왜 0임? */
    //            int enemy_tier = enemyData.Tier;
    //            if (tierSum + enemy_tier < tierSumMax)
    //            {
    //                tierSum += enemy_tier;
    //            }
    //        }
    //        else if (false == TryGetOtherEnemy(tierSumMax - tierSum, out enemyData))
    //        {
    //            //할당 가능한 티어가 없다면 반복문(탐색) 종료
    //            break;
    //        }
    //        else
    //        {
    //            // '그런 유닛 코드는 존재하지 않는다'
    //            Debug.LogError("Wrong unit code");
    //            return;
    //        }

    //        //UnitBattle을 초기화 할 때에 스탯, 모드 등을 일괄 설정한다.
    //        //아하,, 이거 어렵네?
    //        //enemies[index] = new UnitBattle(enemyData);

    //        //count == 1 인데 tier가 많이 남았다면 1번 더 뽑는 것도 좋을텐데
    //        //추가 조건이니까 잠시 보류.
    //    }
    //}
    //private void SelectEnemiesByAscend(MapData mapData, out UnitBattle[] enemies, out int length)
    //{
    //    enemies = new UnitBattle[4];
    //    length = 0;
    //}
    //private void SelectEnemiesByDescend(MapData mapData, out UnitBattle[] enemies, out int length)
    //{
    //    enemies = new UnitBattle[4];
    //    length = 0;
    //}
    //private void SelectEnemiesByTopRated(MapData mapData, out UnitBattle[] enemies, out int length)
    //{
    //    enemies = new UnitBattle[4];
    //    length = 0;
    //}
    //private void SelectEnemiesByLowRated(MapData mapData, out UnitBattle[] enemies, out int length)
    //{
    //    enemies = new UnitBattle[4];
    //    length = 0;
    //}
}

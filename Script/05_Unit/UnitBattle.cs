using DataStruct;
using UnityEngine;
public class UnitBattle : UnitBase
{
    private string      mName;
    private byte[]      mStatBits; //need to use BitManipulator
    private EUnitType   mGroup;
    private byte        mLevel;
    /*(예정) 스킬 정보*/

    public UnitBattle(UnitData data, byte level, Transform transform)
    {
        base.Awake(data.Code, transform);

        mName     = data.Name;
        mGroup    = (EUnitType)data.Group;
        mLevel    = level;
        mStatBits = data.SetBattleStat(mLevel);
    }

    public void SetBattle(Vector3 pos)
    {
        // set position
        transform.localPosition = pos;

        // set animation
        //여기선 "애니메이션을 재생한다"가 맞겠네.
        //next. test battle resource를 만든다. ㅇㅋㅇㅋ...

        // enemy라면? AI 세팅까지.
        if (EUnitType.Enemy == mGroup)
        { 
            // set ai behaviour
        }

        // set active
        gameObject.SetActive(true);
    }
}

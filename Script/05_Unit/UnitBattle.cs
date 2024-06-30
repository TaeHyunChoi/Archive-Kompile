using DataStruct;
using UnityEngine;
public class UnitBattle : UnitBase
{
    private string mName;
    private byte[] mStatBits; //need to use BitManipulator
    private byte   mGroup;
    private byte   mLevel;
    /*(예정) 스킬 정보*/

    public UnitBattle(UnitData data, byte level, Transform transform)
    {
        base.Awake(data.Code, transform);

        mName     = data.Name;
        mGroup    = data.Group;
        mLevel    = level;
        mStatBits = data.SetBattleStat(mLevel);
    }
}

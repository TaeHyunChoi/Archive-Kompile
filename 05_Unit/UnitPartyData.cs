using DataStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Party Unit에 대한 기본적인 정보(unit_code, level, stat, exp, mode)를 저장한다.
/// 레벨업, 기절 등 기본 정보가 변경되면 이 정보를 갱신한다.
/// </summary>
[SerializeField]
public class UnitPartyData
{
    public byte UnitCode { get; private set; }
    public UnitData UnitData { get; private set; }
    public  int[] Statbits { get; private set; }

    public byte Level { get; private set; }
    private int  mExp;
    private int  mMaxExp;

    private byte mModeFlag;

    //private ItemData[]  mEquips; //추후 개발...

    public UnitPartyData(byte codeUnit)
    {
        //현재 게임 정보를 저장하지 않음 -> 임시로 데이터 초기화
        if (false == DataTable.TryGetUnitData(codeUnit, out UnitData data))
        {
            Debug.LogError("null unit data. code: " + codeUnit);
        }

        UnitCode = codeUnit;
        UnitData = data;
        Statbits = new int[data.StatBaseBits.Length];
        data.StatBaseBits.CopyTo(Statbits, 0);
        mExp = 0;
        mMaxExp = 0;
        Level = 1;
        mModeFlag = 0;
    }
}

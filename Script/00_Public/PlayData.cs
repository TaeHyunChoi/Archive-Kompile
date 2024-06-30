using CMathf;
using DataStruct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이 정보를 가진다. (캐릭터 정보, 아이템, 골드, 퀘스트...)
/// 데이터가 자주 바뀔 것으로 예상하여 구조체가 아닌 클래스로 구현
/// </summary>

[SerializeField]
public static class PlayData
{
    public static UnitPartyData[] PartyData { get; set; }
    public static int PartyTier
    {
        get
        {
            int tier = 0;
            int length = PartyData.Length;

            for (int i = 0; i < length; ++i)
            {
                tier += PartyData[i].Level;
            }

            return CMath.FloorToInt(((float)tier / length) / 3, exponent: 3) + 1;
        }
    }

    // field
    public static byte MapCode { get; set; }

    // inventory
    public static int Gold { get; set; }

    public static void LoadData(string dummy)
    {
        if (null != dummy)
        {

        }
        else
        {
            PartyData = new UnitPartyData[1] { new UnitPartyData(1) };
        }
    }
}

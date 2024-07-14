using System;
using static Index.IDxTile;
using CMathf;
using UnityEngine;


public enum EContentType : byte
{
    None = 0,
    Field,
    Battle,
}
public enum ESceneState : byte
{ 
    None,
    Load,
    Play,
    Pause,
    Leave,
}
public enum EUnitType : byte
{ 
    Party = 00,
    Enemy = 10,
    NPC   = 20,
}
public enum EStat
{ 
    HP = 0,
    MP,
    BP,
    STR = 3,
    CON,
    INT,
    WIS,
    DEX,
    AGI,
    LUK,
    CHA,

    Length
}
public enum EAssetType : byte
{ 
    None        = 0,
    AnimCtrl    = 1,
    Prefab      = 2,
    UI          = 3,
    BattleMap   = 4,
}
public enum EPrefabType
{ 
    None        =  0,
    UnitBase    =  1,
    OpeningGame =  2,
}

//해당 자료형 값에 .ToString()하겠다는 뜻으로 접미사 ToString을 붙임
public enum EAnimeCodeToString
{ 
    NONE,

    IDLE_FRONT,
    IDLE_BACK,
    IDLE_LEFT,
    IDLE_RIGHT,

    MOVE_FRONT,
    MOVE_BACK,
    MOVE_LEFT,
    MOVE_RIGHT,
}
[Flags]
public enum EGameStateFlag
{ 
    None    = 0,
    Opening,
    EnterGame, //set field, set battle
    Field,
    Battle,
    Event
}
public enum EUIType : byte
{
    Title = 0,
}

[Flags]
public enum ETileTriggerType : ushort
{
    None = 0,
    Scale = 1 << SHIFT_TRIGGER_SCALE,
    Layer = 1 << SHIFT_TRIGGER_LAYER,
    Event = 1 << SHIFT_TRIGGER_INTERACT
}
public enum ETileSizeType : byte
{
    Default,
    Half,
    Quater,
    Inverse,
    Default_Inverse,
    Half_inverse,
    Quater_inverse
}
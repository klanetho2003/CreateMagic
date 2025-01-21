using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum KeyDownEvent
    {
        N1,
        Q,
        A,
        S,
    }

    public enum Scene
    {

    }

    public enum Sound
    {
        Bgm,
        Effect,
    }

    public enum ObjectType
    {
        Player,
        Monster,
        ProjecTile,
        Env
    }

    public enum MonsterID
    {
        Goblin = 1,
        Snake = 2,
        Boss = 3,
    }

    public enum SkillType
    {
        None,
        Sequence,
        Repeat, //탕탕이라 존재 하는 것 (애초에 반복적으로 호출하는 것은 스킬 사용자가 반복 호출을 하는 것이지 스킬 스스로가 정의하고 있어야 할 필요가 없기 때문)
        Single,
        Iner,
    }

    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }

    public enum StageType
    {
        Normal,
        Boss,
    }

    public enum CreatureState
    {
        Idle,
        Moving,
        Casting,
        DoSkill,
        Dameged,
        Dead,
    }

    public const int PLAYER_DATA_ID = 1;
    public const string EXP_JAM_PREFAB = "EXPJam.prefab";

    public const string Spawner_ID = "_Generater";
    public const string Fire_Ball_ID = "N1QA";
    public const string CastingImapct_ID = "N1";
    public const string EGO_SWORD_ID = "10";
}

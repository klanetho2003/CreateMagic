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
        Repeat, //�����̶� ���� �ϴ� �� (���ʿ� �ݺ������� ȣ���ϴ� ���� ��ų ����ڰ� �ݺ� ȣ���� �ϴ� ������ ��ų �����ΰ� �����ϰ� �־�� �� �ʿ䰡 ���� ����)
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

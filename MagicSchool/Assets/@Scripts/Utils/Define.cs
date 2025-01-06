using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum KeyEvent
    {
        KeyDown_1,
        KeyDown_Q,
        KeyDown_A,
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
    }

    public enum SkillID
    {
        None = 0,
        Fire_Ball_ID = 1,
        EGO_SWORD_ID = 10,
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
        Skill,
        Dead,
    }

    public const int PLAYER_DATA_ID = 1;
    public const string EXP_JAM_PREFAB = "EXPJam.prefab";
    public const int EGO_SWORD_ID = 10;
}

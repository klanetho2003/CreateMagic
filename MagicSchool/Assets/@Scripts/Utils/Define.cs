using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EObjectType
    {
        None,
        Creature,
        Skill,
        ProjecTile,
        Env
    }

    public enum ECreatureType
    {
        None,

        Student,
        Monster,
    }

    public enum ESkillType
    {
        None,
        Repeat,
        Single,
        Sequence,
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

    public enum KeyDownEvent
    {
        N1,
        N2,
        N3,
        N4,
        Q,
        W,
        E,
        R,
        A,
        S,
        D,
    }

    public enum Scene
    {

    }

    public enum Sound
    {
        Bgm,
        Effect,
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


    public const string EXP_JAM_PREFAB = "EXPJam.prefab";
    
    public const string Fire_Ball_ID = "N1QA";
    public const string CastingImapct_ID = "N1";
    public const string EGO_SWORD_ID = "10";
    public const string Spawner_ID = "_Generater";

    #region Creatrue ID
    public const int MAGICION01_ID = 201000;
    public const int KNIGHT01_ID = 201001;

    public const int MONSTER_ASSASIN_ID = 202001;
    public const int MONSTER_SKELETON_ID = 202002;
    public const int MONSTER_SKELETO_SHIELD_ID = 202003;
    public const int MONSTER_MAGESKELETON_ID = 202004;
    public const int MONSTER_MAGESKELETON_SHIELD_ID = 202005;
    #endregion

    //HARD CODING
    public const int MONSTER_DEFAULT_MELEE_ATTACK_RANGE = 1;
    public const int MONSTER_DEFAULT_RANGED_ATTACK_RANGE = 4;

    public static class AnimationName
    {
        public const string IDLE = "Idle";
        public const string MOVING = "Moving";
        public const string CASTING = "Casting";
        public const string DOSKILL = "DoSkill";
        public const string DAMAGED = "Damaged";
        public const string DEAD = "Death";
    }

    public static class SortingLayers
    {
        public const int SPELL_INDICATOR = 200;
        public const int CREATURE = 300;
        public const int ENV = 300;
        public const int PROJECTILE = 310;
        public const int SKILL_EFFECT = 310;
        public const int DAMAGE_FONT = 410;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum ESkillType
    {
        None,
        Repeat,
        Single,
        Sequence,
    }

    public const string EXP_JAM_PREFAB = "EXPJam.prefab";

    #region Manager
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum KeyDownEvent
    {
        N1 = 1,
        N2 = 2,
        N3 = 3,
        N4 = 4,
        N5 = 5,
        N6 = 6,
        Q = 7,
        W = 8,
        E = 9,
        R = 0,
        A = 11,
        S = 12,
        D = 13,
        space = 20,
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
    #endregion

    #region Object

    // Creatrue ID
    public const int MAGICION01_ID = 201000;
    public const int KNIGHT01_ID = 201001;

    public const int MONSTER_ASSASIN_ID = 202001;
    public const int MONSTER_SKELETON_ID = 202002;
    public const int MONSTER_SKELETO_SHIELD_ID = 202003;
    public const int MONSTER_MAGESKELETON_ID = 202004;
    public const int MONSTER_MAGESKELETON_SHIELD_ID = 202005;

    public enum EObjectType
    {
        None,
        Student,
        Monster,
        Npc,
        ProjecTile,
        Env,
        Effect,
        ItemHolder,
    }

    public enum ENpcType
    {
        Camp,
        Portal,
        Waypoint,
        BlackSmith,
        Guild,
        TreasureBox,
        Dungeon,
        WaveCheat,
        Quest,
    }

    public enum CreatureState
    {
        Spawning,
        Idle,
        Moving,
        Casting,
        FrontDelay,
        DoSkill,
        BackDelay,
        Dameged,
        Stun,
        Dead,
    }

    public enum EMonsterWaveType
    {
        Standby,
        First,
        Second,
    }

    public enum ELayer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Dummy1 = 3,
        Water = 4,
        UI = 5,
        Student = 6,
        Monster = 7,
        Env = 8,
        Obstacle = 9,
        Projectile = 10,
    }

    public enum EColliderSize
    {
        Small,
        Normal,
        Big,
    }

    public enum ECellCollisionType
    {
        None,
        SemiWall,
        Wall,
    }
    #endregion

    #region Effect
    public enum EEffectSize
    {
        CircleSmall,
        CircleNormal,
        CircleBig,
        ConeSmall,
        ConeNormal,
        ConeBig,
    }

    public enum EEffectType
    {
        Buff,
        Debuff,
        CrowdControl, // CC
    }

    public enum EEffectSpawnType
    {
        Skill, // 지속시간이 있는 기본적인 이펙트 
        External, // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음)
    }

    public enum EEffectClearType
    {
        TimeOut, // 시간초과로 인한 Effect 종료
        ClearSkill, // 정화 스킬로 인한 Effect 종료
        TriggerOutAoE, // AoE스킬을 벗어난 종료
        EndOfAirborne, // 에어본이 끝난 경우 호출되는 종료
        Despawn, // Clear 시 종료
    }

    public enum EEffectClassName
    {
        Bleeding,
        Poison,
        Ignite,
        Heal,
        AttackBuff,
        MoveSpeedBuff,
        AttackSpeedBuff,
        LifeStealBuff,
        ReduceDmgBuff,
        ThornsBuff,
        Knockback,
        Airborne,
        PullEffect,
        Stun,
        Freeze,
        CleanDebuff,
    }
    #endregion

    #region Item
    public enum EItemGrade
    {
        None,
        Normal,
        Rare,
        Epic,
        Legendary
    }

    public enum EItemGroupType // Remmove 해도 될 듯
    {
        None,
        Equipment,
        StatBoost,
        Consumable,
    }

    public enum EItemType
    {
        None,
        Artifact,
        Weapon,
        Armor,
        Potion,
        Scroll
    }

    public enum EItemSubType
    {
        None,

        Sword,
        Dagger,
        Bow,

        Helmet,
        Armor,
        Shield,
        Gloves,
        Shoes,

        EnchantWeapon,
        EnchantArmor,

        HealthPotion,
        ManaPotion,
    }

    public enum EEquipSlotType
    {
        None,
        Weapon = 1,
        Helmet = 2,
        Armor = 3,
        Shield = 4,
        Gloves = 5,
        Shoes = 6,
        EquipMax,

        Inventory = 100,
        UnknownItems = 200,
    }
    #endregion

    #region Quest
    public enum EQuestPeriodType
    {
        Once, // 단발성
        Daily,
        Weekly,
        Infinite, // 무한으로
    }

    public enum EQuestCondition
    {
        None,
        Level,
        ItemLevel,

    }

    public enum EQuestObjectiveType
    {
        KillMonster,
        // Item 통합 고려
        EarnMeat,
        SpendMeat,
        EarnWood,
        SpendWood,
        EarnMineral,
        SpendMineral,
        EarnGold,
        SpendGold,
        //
        UseItem,
        Survival,
        ClearDungeon
    }

    public enum EQuestRewardType
    {
        Hero,
        Gold,
        Mineral,
        Meat,
        Wood,
        Item,
    }

    public enum EQuestState
    {
        None, // UnKnown - 받지도 않음
        Processing,
        Completed,
        Rewarded,
    }

    public enum EResourceType
    {
        Wood,
        Mineral,
        Meat,
        Gold,
        Materials,
        Dia
    }

    public enum EProviderType
    {
        None = 0,
        Guest = 1,
        Google = 2,
        Facebook = 3,
    }
    #endregion

    public enum EBroadcastEventType
    {
        None,
        ChangeMeat,
        ChangeWood,
        ChangeMineral,
        ChangeGold,
        ChangeDia,
        ChangeMaterials,
        KillMonster,
        LevelUp,
        DungeonClear,
        ChangeInventory,
        ChangeCrew,
        QuestClear,
    }

    public enum ESkillSlot
    {
        Default,
        Env,
        A,
        B,
    }

    public enum EIndicatorType
    {
        None,
        Cone,
        Rectangle,
    }

    public enum EFindPathResult
    {
        Fail_LerpCell,
        Fail_NoPath,
        Fail_MoveTo,
        Success,
    }

    public enum EStatModType
    {
        None,
        Add,
        PercentAdd,
        PercentMult,
    }

    public enum EMpStateType
    {
        Fill,
        Full,
        None,
    }

    public enum ELanguage
    {
        Korean,
        English,
        Japanese,
    }

    //HARD CODING
    public const float EFFECT_SMALL_RADIUS = 1f;
    public const float EFFECT_NORMAL_RADIUS = 2.5f;
    public const float EFFECT_BIG_RADIUS = 5.5f;

    public const float EFFECT_KNOCKBACK_SPEED = 25f;

    public const int MONSTER_DEFAULT_MELEE_ATTACK_RANGE = 1;
    public const int MONSTER_DEFAULT_RANGED_ATTACK_RANGE = 3;
    public const int INIT_SKILLKEY = 0;
    public const int PROJECTILE_DISTANCE_MAX = 1000;

    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1';
    public const char MAP_TOOL_SEMI_WALL = '2';

    public const int PLAYER_DEFAULT_MOVE_DEPTH = 5;
    public const int MONSTER_DEFAULT_MOVE_DEPTH = 10;

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
        public const int NPC = 310;
        public const int PROJECTILE = 310;
        public const int SKILL_EFFECT = 310;
        public const int DAMAGE_FONT = 410;
    }
}

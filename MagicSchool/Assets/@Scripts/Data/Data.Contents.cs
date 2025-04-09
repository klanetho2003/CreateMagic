using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;
using static Define;

namespace Data
{
    #region Temp : Dev Memo

    #region Player Data
    /*public class PlayerData
    {
        public int templateID;
        public int maxHp;
        public int maxMp;

        public int speed;

        public int attack;
        public int totalExp;
    }
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        public List<PlayerData> stats = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
            foreach (PlayerData stat in stats)
                dict.Add(stat.templateID, stat);
            return dict;
        }
    }*/
    #endregion

    #region Player Xml Data
    /*public class PlayerData
	{
        [XmlAttribute]
        public int templateID;
        [XmlAttribute]
        public int maxHp;
        [XmlAttribute]
        public int maxMp;

        [XmlAttribute]
		public int speed;

		[XmlAttribute]
		public int attack;
		[XmlAttribute]
		public int totalExp;
	}

	[Serializable, XmlRoot("PlayerDatas")]
	public class PlayerDataLoader : ILoader<int, PlayerData>
	{
		[XmlElement("PlayerData")]
		public List<PlayerData> stats = new List<PlayerData>();

		public Dictionary<int, PlayerData> MakeDict()
		{
			Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
			foreach (PlayerData stat in stats)
				dict.Add(stat.templateID, stat);
			return dict;
		}
	}*/
    #endregion

    #region Monster Xml Data
    /*public class MonsterData
    {
        [XmlAttribute]
        public int templateID;
        [XmlAttribute]
		public string name;
		[XmlAttribute]
		public string prefab;
		[XmlAttribute]
		public int level;
		[XmlAttribute]
		public int maxHp;
		[XmlAttribute]
		public int attack;
		[XmlAttribute]
		public float speed;
		// DropData
		// - 일정 확률로
		// - 어떤 아이템을 (보석, 스킬 가차, 골드, 고기)
		// - 몇 개 드랍할지?
	}

    [Serializable, XmlRoot("MonsterDatas")]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        [XmlElement("MonsterData")]
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.templateID, monster);
            return dict;
        }
    }*/
    #endregion

    #region Skill Xml Data
    /*public class SkillData
	{
		[XmlAttribute]
		public string templateID;
		[XmlAttribute]
		public string name;
		//[XmlAttribute]
		//public string skillTypeStr;
		//public Define.SkillType type = Define.SkillType.None;

		[XmlAttribute]
		public string prefab;

        [XmlAttribute]
        public float activateSkillDelay;
        [XmlAttribute]
        public float completeSkillDelay;
        [XmlAttribute]
		public int damage;
		[XmlAttribute]
		public int speed;
	}*/

    /*[Serializable, XmlRoot("SkillDatas")]
	public class SkillDataLoader : ILoader<string, SkillData>
	{
		[XmlElement("SkillData")]
		public List<SkillData> skills = new List<SkillData>();

		public Dictionary<string, SkillData> MakeDict()
		{
			Dictionary<string, SkillData> dict = new Dictionary<string, SkillData>();
			foreach (SkillData skill in skills)
				dict.Add(skill.templateID, skill);
			return dict;
		}
	}*/
    #endregion

    #endregion


    #region Creature Data
    public class CreatureData //public string prefab; // 이제 Prefab 경로는 불필요
    {
        public int DataId;
        public string DescriptionTextID;
        public string Label;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float AtkRange;
        public float SpawnDelaySeconds;
        public string IconImage;
        public string MaterialID;
        public string AnimatorDataID;
        public string SortingLayerName;
    }
    #endregion

    #region Monster Data
    [Serializable]
    public class MonsterData : CreatureData //public string prefab; // 이제 Prefab 경로는 불필요
    {
        public int DefaultSkillId;
        public int SkillAId;
        public int SkillBId;
        public int DropItemId;
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.DataId, monster);
            return dict;
        }
    }
    #endregion

    #region Student Data
    [Serializable]
    public class StudentData : CreatureData //public string prefab; // 이제 Prefab 경로는 불필요
    {
        // 추가되는 부분
        public int N1_DefaultSkillId;
        public int N2_DefaultSkillId;
        public int N3_DefaultSkillId;
        public int N4_DefaultSkillId;

        public int Q_DefaultSkillId;
        public int W_DefaultSkillId;
        public int E_DefaultSkillId;
        public int R_DefaultSkillId;

        public int A_DefaultSkillId;
        public int S_DefaultSkillId;
        public int D_DefaultSkillId;

        public List<int> TempDevSkillsDataId; // 초기 데이터 시트로 이전 필요

        public int MaxMp;
        public float MpGaugeAmount;
    }

    [Serializable]
    public class StudentDataLoader : ILoader<int, StudentData>
    {
        public List<StudentData> Students = new List<StudentData>();
        public Dictionary<int, StudentData> MakeDict()
        {
            Dictionary<int, StudentData> dict = new Dictionary<int, StudentData>();
            foreach (StudentData Student in Students)
                dict.Add(Student.DataId, Student);
            return dict;
        }
    }
    #endregion

    #region NPC
    [Serializable]
    public class NpcData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextID;
        public ENpcType NpcType;
        public string PrefabLabel;
        public string IconImage;
        public string SortingLayerName;
        public string AnimatorDataID;
        public int QuestDataId;
    }

    [Serializable]
    public class NpcDataLoader : ILoader<int, NpcData>
    {
        public List<NpcData> creatures = new List<NpcData>();
        public Dictionary<int, NpcData> MakeDict()
        {
            Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
            foreach (NpcData creature in creatures)
                dict.Add(creature.DataId, creature);
            return dict;
        }
    }
    #endregion

    #region Creature Stat Data
    public class CreatureStatData
    {
        public int CreatureDataId;
        public float MaxHp;
        public float Atk;
        public float MoveSpeed;
        public float CriRate;
        public float CriDamageMult;
    }
    #endregion

    #region Monster Stat Data
    [Serializable]
    public class MonsterStatData : CreatureStatData
    {
        // To Do
    }

    [Serializable]
    public class MonsterStatDataLoader : ILoader<int, MonsterStatData>
    {
        public List<MonsterStatData> monsterStats = new List<MonsterStatData>();
        public Dictionary<int, MonsterStatData> MakeDict()
        {
            Dictionary<int, MonsterStatData> dict = new Dictionary<int, MonsterStatData>();
            foreach (MonsterStatData monsterStat in monsterStats)
                dict.Add(monsterStat.CreatureDataId, monsterStat);
            return dict;
        }
    }
    #endregion

    #region Student Stat Data
    [Serializable]
    public class StudentStatData : CreatureStatData
    {
        // To Do
    }

    [Serializable]
    public class StudentStatDataLoader : ILoader<int, StudentStatData>
    {
        public List<StudentStatData> studentStats = new List<StudentStatData>();
        public Dictionary<int, StudentStatData> MakeDict()
        {
            Dictionary<int, StudentStatData> dict = new Dictionary<int, StudentStatData>();
            foreach (StudentStatData studentStat in studentStats)
                dict.Add(studentStat.CreatureDataId, studentStat);
            return dict;
        }
    }
    #endregion


    #region Skill Data

    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string Description;
        public int ProjectileId;
        public string PrefabLabel;
        public string IconLabel;
        public string AnimName;
        public float DamageMultiplier;
        public float SkillDuration;
        public string CastingSound;
        public float SkillRange;
        public int TargetCount;
        public List<int> EffectIds = new List<int>();
        public int AoEId;
        public float RangeMultipleX;
        public float RangeMultipleY;
        public EEffectSize EffectSize;
        public float ActivateSkillDelay;
    }

    #endregion

    #region Monster Skill Data

    [Serializable]
    public class MonsterSkillData : SkillData
    {
        public float CoolTime;
        public float AnimImpactDuration;
    }

    [Serializable]
    public class MonsterSkillDataLoader : ILoader<int, MonsterSkillData>
    {
        public List<MonsterSkillData> monsterSkills = new List<MonsterSkillData>();
        public Dictionary<int, MonsterSkillData> MakeDict()
        {
            Dictionary<int, MonsterSkillData> dict = new Dictionary<int, MonsterSkillData>();
            foreach (MonsterSkillData monsterSkill in monsterSkills)
                dict.Add(monsterSkill.DataId, monsterSkill);
            return dict;
        }
    }

    #endregion

    #region Player Skill Data

    [Serializable]
    public class PlayerSkillData : SkillData
    {
        public List<int> InputValues = new List<int>();
        public string InputDescription;
        public int UsedMp;
    }

    [Serializable]
    public class PlayerSkillDataLoader : ILoader<int, PlayerSkillData>
    {
        public List<PlayerSkillData> playerSkills = new List<PlayerSkillData>();
        public Dictionary<int, PlayerSkillData> MakeDict()
        {
            Dictionary<int, PlayerSkillData> dict = new Dictionary<int, PlayerSkillData>();
            foreach (PlayerSkillData playerSkill in playerSkills)
                dict.Add(playerSkill.DataId, playerSkill);
            return dict;
        }
    }

    #endregion

    #region SkillInfoData - To Do Skill과 매칭 되도록 Sheet부터 수정 필요
    [Serializable]
    public class SkillInfoData
    {
        public int DataId;
        public string NameTextId;
        public string DescriptionTextId;
        public string Rarity;
        public float GachaSpawnWeight;
        public float GachaWeight;
        public int GachaExpCount;
        public string IconImage;
    }

    [Serializable]
    public class SkillInfoDataLoader : ILoader<int, SkillInfoData>
    {
        public List<SkillInfoData> heroInfo = new List<SkillInfoData>();
        public Dictionary<int, SkillInfoData> MakeDict()
        {
            Dictionary<int, SkillInfoData> dict = new Dictionary<int, SkillInfoData>();
            foreach (SkillInfoData info in heroInfo)
                dict.Add(info.DataId, info);
            return dict;
        }
    }
    #endregion

    #region Projetile Data

    [Serializable]
    public class ProjectileData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string ComponentName;
        public string MaterialID;
        public string AnimatorDataID;
        public string PrefabLabel;
        public float Duration;  // 왜 올렸는데 적용이 안 돼요? 
        public float HitSound;
        public float ProjRange; //아 그거 특정 skill 에서만 되는 거에요.
        public float ProjSpeed;
        public float ReserveDestroyTime;
    }

    [Serializable]
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> projectiles = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData projectile in projectiles)
                dict.Add(projectile.DataId, projectile);
            return dict;
        }
    }

    #endregion

    #region EffectData
    [Serializable]
    public class EffectData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string DescriptionTextID;
        public string AnimatorDataID;
        public string AnimName;
        public string MaterialID;
        public string SpriteID;
        public string SortingLayerName;
        public string IconLabel;
        public string SoundLabel;
        public float Amount;
        public float PercentAdd;
        public float PercentMult;
        public float TickTime;
        public float TickCount;
        public EEffectType EffectType;
    }

    [Serializable]
    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> effects = new List<EffectData>();
        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
            foreach (EffectData effect in effects)
                dict.Add(effect.DataId, effect);
            return dict;
        }
    }
    #endregion

    #region AoEData
    [Serializable]
    public class AoEData
    {
        public int DataId;
        public string Name;
        public string PrefabLabel;
        public string ClassName;
        public string AnimatorDataID;
        public string SoundLabel;
        public float Duration;
        public List<int> AllyEffects = new List<int>();
        public List<int> EnemyEffects = new List<int>();
        public string AnimName;
        public string SortingLayerName;
    }

    [Serializable]
    public class AoEDataLoader : ILoader<int, AoEData>
    {
        public List<AoEData> aoes = new List<AoEData>();
        public Dictionary<int, AoEData> MakeDict()
        {
            Dictionary<int, AoEData> dict = new Dictionary<int, AoEData>();
            foreach (AoEData aoe in aoes)
                dict.Add(aoe.DataId, aoe);
            return dict;
        }
    }
    #endregion

    #region TextData
    [Serializable]
    public class TextData
    {
        public string DataId;
        public string KOR;
    }

    [Serializable]
    public class TextDataLoader : ILoader<string, TextData>
    {
        public List<TextData> texts = new List<TextData>();
        public Dictionary<string, TextData> MakeDict()
        {
            Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
            foreach (TextData text in texts)
                dict.Add(text.DataId, text);
            return dict;
        }
    }
    #endregion

    #region Item
    // 계층 구조 예시
    // Equipment.Weapon.Dagger
    // Consumable.Potion.Hp
    [Serializable]
    public class BaseData
    {
        public int DataId;
    }

    [Serializable]
    public class ItemData : BaseData
    {
        public string Name;
        public string SpriteName;
        public EItemGroupType ItemGroupType;
        public EItemType Type;
        public EItemSubType SubType;
        public EItemGrade Grade;
        public EStatModType StatModType;
        public int MaxStack;
    }

    [Serializable]
    public class ArtifactData : ItemData
    {
        public float Damage;
        public float Defence;
        public float Speed;
    }

    [Serializable]
    public class EquipmentData : ItemData
    {
        public float Damage;
        public float Defence;
        public float Speed;
    }

    [Serializable]
    public class ConsumableData : ItemData
    {
        public double Value;
        public int CoolTime;
    }

    [Serializable]
    public class ItemDataLoader<T> : ILoader<int, T> where T : BaseData
    {
        public List<T> items = new List<T>();

        public Dictionary<int, T> MakeDict()
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();
            foreach (T item in items)
                dict.Add(item.DataId, item);

            return dict;
        }
    }
    #endregion

    #region DropTable

    [Serializable]
    public class DropTableData
    {
        public int DataId;
        public int RewardExp;
        public List<RewardData> Rewards = new List<RewardData>();
    }

    [Serializable]
    public class RewardData
    {
        public int Probability; // 100분율
        public int ItemTemplateId;
        // public int Count;
    }

    [Serializable]
    public class DropTableDataLoader : ILoader<int, DropTableData>
    {
        public List<DropTableData> dropTables = new List<DropTableData>();

        public Dictionary<int, DropTableData> MakeDict()
        {
            Dictionary<int, DropTableData> dict = new Dictionary<int, DropTableData>();
            foreach (DropTableData tables in dropTables)
                dict.Add(tables.DataId, tables);
            return dict;
        }
    }

    #endregion

    #region Quest Data

    [Serializable]
    public class QuestData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextId;
        public EQuestPeriodType QuestPeriodType;
        // public EQuestCondition Condition // Quest 등장 조건 ex. Level
        public List<QuestTaskData> QuestTasks = new List<QuestTaskData>();
        public List<QuestRewardData> Rewards = new List<QuestRewardData>(); // To Do. Reward Data 통일
    }

    [Serializable]
    public class QuestTaskData
    {
        public EQuestObjectiveType ObjectiveType;   // 목적
        // public string DescriptionTextId;           // "{0}"를 처치하세요 -> Use String Format
        public int ObjectiveDataId;
        public int ObjectiveCount;
    }

    [Serializable]
    public class QuestRewardData
    {
        public EQuestRewardType RewardType;
        public int RewardDataId;
        public int RewardCount;
    }

    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public List<QuestData> quests = new List<QuestData>();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
            foreach (QuestData quest in quests)
                dict.Add(quest.DataId, quest);
            return dict;
        }
    }

    #endregion
}

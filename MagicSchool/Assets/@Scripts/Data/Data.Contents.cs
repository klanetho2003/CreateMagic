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
        #region Stat
        public float MaxHp;
        public float UpMaxHp;

        public int MaxMp;
        public float MpGaugeAmount;

        public float Atk;
        public float AtkRange;
        public float AtkBonus;

        public float MoveSpeed;

        public float CriRate;
        public float Cridamage;
        #endregion
        public float SpawnDelaySeconds;
        public string IconImage;
        public string MaterialID;
        public string AnimatorDataID;
        public string SortingLayerName;
        public int DefaultSkillId;
        public int EnvSkillId;
        public int SkillAId;
        public int SkillBId;
    }
    #endregion

    #region Monster Data
    [Serializable]
    public class MonsterData : CreatureData //public string prefab; // 이제 Prefab 경로는 불필요
    {
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

    #region Skill Data

    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string Description;
        public List<int> InputValues = new List<int>();
        public string InputDescription;
        public int ProjectileId;
        public string PrefabLabel;
        public string IconLabel;
        public string AnimName;
        public int UsedMp;
        public float CoolTime;
        public float DamageMultiplier;
        public float SkillDuration;
        public float AnimImpactDuration;
        public string CastingSound;
        public float SkillRange;
        public float ScaleMultiplier;
        public int TargetCount;
        public List<int> EffectIds = new List<int>();
        public int NextLevelId;
        public int AoEId;
        public EEffectSize EffectSize;
        public float ActivateSkillDelay;
        public float CompleteSkillDelay;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();
        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.DataId, skill);
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

    #region NPC
    [Serializable]
    public class NpcData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextID;
        public ENpcType NpcType;
        public string PrefabLabel;
        public string SpriteName;
        public string SortingLayerName;
        public string AnimatorDataID;
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

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
        public int TemplateID;

        public string DescriptionTextID;
        public string Label;

        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;

        public float Mass;

        #region Stat
        public float MaxHp;
        public float UpMaxHp;

        public float Atk;
        public float AtkRange;
        public float AtkBonus;

        public float Def;
        public float MoveSpeed;

        public float TotalExp;

        public float HpRate;
        public float AtkRate;
        public float DefRate;
        public float MoveSpeedRate;
        #endregion

        public string IconImage;
        public string MaterialID;
        public string AnimatorDataID;
        public string SortingLayerName
;
        public List<int> SkillList;

        public float DropItemId;
        public float StandardAttack;
    }
    #endregion

    #region Monster Data
    [Serializable]
    public class MonsterData : CreatureData //public string prefab; // 이제 Prefab 경로는 불필요
    {
        // 추가되는 부분
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();
        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.TemplateID, monster);
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
                dict.Add(Student.TemplateID, Student);
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
        public string ComponentName;
        public string Description;
        public int ProjectileId;
        public string PrefabLabel;
        public string IconLabel;
        public string AnimName;
        public float CoolTime;
        public float DamageMultiplier;
        public float Duration;
        public float NumProjectiles;
        public string CastingSound;
        public float AngleBetweenProj;
        public float SkillRange;
        public float RotateSpeed;
        public float ScaleMultiplier;
        public float AngleRange;

        public List<int> NextSkills;
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

    #region Projetile Data

    [Serializable]
    public class ProjectileData
    {
        public int DataId;
        public string Name;
        public string ComponentName;
        public string MaterialID;
        public string AnimatorDataID;
        public string PrefabLabel;
        public float Duration;  // 왜 올렸는데 적용이 안 돼요? 
        public float NumBounce;
        public float NumPenerations;
        public float HitSound;
        public float ProjRange; //아 그거 특정 skill 에서만 되는 거에요.
        public float ProjSpeed;
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
}

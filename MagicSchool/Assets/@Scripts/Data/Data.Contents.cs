using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Data
{
    #region Player Data
    public class PlayerData
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
    }
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

    #region MonsterData
    public class MonsterData
    {
        public int templateID;

		public string name;
		public string prefab;

		public int level;
		public int maxHp;
		public int attack;
		public float speed;
		// DropData
		// - 일정 확률로
		// - 어떤 아이템을 (보석, 스킬 가차, 골드, 고기)
		// - 몇 개 드랍할지?
	}

    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.templateID, monster);
            return dict;
        }
    }
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

    #region SkillData

    [Serializable]
    public class SkillData
    {
        public string templateID;
        public string name;
        public string prefab;
        public string type;

        public float activateSkillDelay;
        public float completeSkillDelay;

        public int damage;
        public int speed;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<string, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();


        public Dictionary<string, SkillData> MakeDict()
        {
            Dictionary<string, SkillData> dict = new Dictionary<string, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.templateID, skill);
            return dict;
        }
    }

    #endregion
    #region SkillData Xml 주석
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
}

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.InitGameData> InitGameDic { get; private set; } = new Dictionary<int, Data.InitGameData>();

    public Dictionary<int, Data.MonsterData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.StudentData> StudentDic { get; private set; } = new Dictionary<int, Data.StudentData>();
    public Dictionary<int, Data.NpcData> NpcDic { get; private set; } = new Dictionary<int, Data.NpcData>();

    public Dictionary<int, Data.MonsterStatData> MonsterStatDic { get; private set; } = new Dictionary<int, Data.MonsterStatData>();
    public Dictionary<int, Data.StudentStatData> StudentStatDic { get; private set; } = new Dictionary<int, Data.StudentStatData>();
    
    public Dictionary<int, Data.StudentSkillData> StudentSkillDic { get; private set; } = new Dictionary<int, Data.StudentSkillData>();
    public Dictionary<int, Data.MonsterSkillData> MonsterSkillDic { get; private set; } = new Dictionary<int, Data.MonsterSkillData>();
    public Dictionary<int, Data.SkillInfoData> SkillInfoDic { get; private set; } = new Dictionary<int, Data.SkillInfoData>();

    public Dictionary<int, Data.ProjectileData> ProjectileDic { get; private set; } = new Dictionary<int, Data.ProjectileData>();
    public Dictionary<int, Data.EffectData> EffectDic { get; private set; } = new Dictionary<int, Data.EffectData>();
    public Dictionary<int, Data.AoEData> AoEDic { get; private set; } = new Dictionary<int, Data.AoEData>();

    public Dictionary<int, Data.StatBoostData> StatBoostsDic { get; private set; } = new Dictionary<int, Data.StatBoostData>();
    public Dictionary<int, Data.EquipmentData> EquipmentsDic { get; private set; } = new Dictionary<int, Data.EquipmentData>();
    public Dictionary<int, Data.ConsumableData> ConsumableDic { get; private set; } = new Dictionary<int, Data.ConsumableData>();
    public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.ItemProbabilityData> ItemProbabilityDic { get; private set; } = new Dictionary<int, Data.ItemProbabilityData>();
    public Dictionary<int, Data.DropTableData> DropTableDic { get; private set; } = new Dictionary<int, Data.DropTableData>();


    public Dictionary<int, Data.QuestData> QuestDic { get; private set; } = new Dictionary<int, Data.QuestData>();

    public Dictionary<string, Data.TextData> TextDic { get; private set; } = new Dictionary<string, Data.TextData>();

    //public Dictionary<string, Data.PlayerSkillData> PlayerSkillDic { get; private set; } = new Dictionary<string, Data.PlayerSkillData>();

    public void Init()
    {
        InitGameDic = LoadJson<Data.InitGameDataLoader, int, Data.InitGameData>("InitGameData").MakeDict();

        #region Creature
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        StudentDic = LoadJson<Data.StudentDataLoader, int, Data.StudentData>("StudentData").MakeDict();
        NpcDic = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();

        MonsterStatDic = LoadJson<Data.MonsterStatDataLoader, int, Data.MonsterStatData>("MonsterStatData").MakeDict();
        StudentStatDic = LoadJson<Data.StudentStatDataLoader, int, Data.StudentStatData>("StudentStatData").MakeDict();
        #endregion

        #region Skill
        StudentSkillDic = LoadJson<Data.StudentSkillDataLoader, int, Data.StudentSkillData>("StudentSkillData").MakeDict();
        MonsterSkillDic = LoadJson<Data.MonsterSkillDataLoader, int, Data.MonsterSkillData>("MonsterSkillData").MakeDict();
        SkillInfoDic = LoadJson<Data.SkillInfoDataLoader, int, Data.SkillInfoData>("SkillInfoData").MakeDict();

        ProjectileDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();

        EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();
        AoEDic = LoadJson<Data.AoEDataLoader, int, Data.AoEData>("AoEData").MakeDict();
        #endregion

        #region Item
        StatBoostsDic = LoadJson<Data.ItemDataLoader<Data.StatBoostData>, int, Data.StatBoostData>("Item_StatBoostData").MakeDict();
        EquipmentsDic = LoadJson<Data.ItemDataLoader<Data.EquipmentData>, int, Data.EquipmentData>("Item_EquipmentData").MakeDict();
        ConsumableDic = LoadJson<Data.ItemDataLoader<Data.ConsumableData>, int, Data.ConsumableData>("Item_ConsumableData").MakeDict();
        ItemProbabilityDic = LoadJson<Data.ItemProbabilityDataLoader, int, Data.ItemProbabilityData>("ItemProbabilityData").MakeDict();
        DropTableDic = LoadJson<Data.DropTableDataLoader, int, Data.DropTableData>("DropTableData").MakeDict();

        ItemDic.Clear();
        foreach (var item in StatBoostsDic)
            ItemDic.Add(item.Key, item.Value);

        foreach (var item in EquipmentsDic)
            ItemDic.Add(item.Key, item.Value);

        foreach (var item in ConsumableDic)
            ItemDic.Add(item.Key, item.Value);
        #endregion

        // Quest
        QuestDic = LoadJson<Data.QuestDataLoader, int, Data.QuestData>("QuestData").MakeDict();

        // Languge
        TextDic = LoadJson<Data.TextDataLoader, string, Data.TextData>("TextData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    Item LoadSingleXml<Item>(string name)
	{
		XmlSerializer xs = new XmlSerializer(typeof(Item));
		TextAsset textAsset = Managers.Resource.Load<TextAsset>(name);
		using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			return (Item)xs.Deserialize(stream);
	}

	Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
	{
		XmlSerializer xs = new XmlSerializer(typeof(Loader));
		TextAsset textAsset = Managers.Resource.Load<TextAsset>(name);
		using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(textAsset.text)))
			return (Loader)xs.Deserialize(stream);
	}
}

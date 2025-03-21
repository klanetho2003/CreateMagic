using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
    [MenuItem("Tools/RemoveSaveData")]
    public static void RemoveSaveData()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("SaveFile Deleted");
        }
        else
        {
            Debug.Log("No SaveFile Detected");
        }
    }

    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcelDataToJson()
    {
        ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
        ParseExcelDataToJson<StudentDataLoader, StudentData>("Student");
        ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
        ParseExcelDataToJson<SkillInfoDataLoader, SkillInfoData>("SkillInfo");
        ParseExcelDataToJson<ProjectileDataLoader, ProjectileData>("Projectile");
        ParseExcelDataToJson<EffectDataLoader, EffectData>("Effect");
        ParseExcelDataToJson<AoEDataLoader, AoEData>("AoE");
        ParseExcelDataToJson<NpcDataLoader, NpcData>("Npc");
        ParseExcelDataToJson<TextDataLoader, TextData>("Text");

        ParseExcelDataToJson<ItemDataLoader<EquipmentData>, EquipmentData>("Item_Equipment");
        ParseExcelDataToJson<ItemDataLoader<ConsumableData>, ConsumableData>("Item_Consumable");

        ParseExcelDataToJson<DropTableDataLoader, DropTableData_Internal>("DropTable");


        //ParseExcelDataToJson<PlayerSkillDataLoader, PlayerSkillData>("PlayerSkill");

        //LEGACY_ParseTestData("Test");

        Debug.Log("DataTransformer Completed");
    }

    #region LEGACY
    // LEGACY !
    /*public static T ConvertValue<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return default(T);

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        return (T)converter.ConvertFromString(value);
    }

    public static List<T> ConvertList<T>(string value)
    {
        if (string.IsNullOrEmpty(value))
            return new List<T>();

        return value.Split('&').Select(x => ConvertValue<T>(x)).ToList();
    }

    static void LEGACY_ParseTestData(string filename)
    {
        TestDataLoader loader = new TestDataLoader();

        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

        for (int y = 1; y < lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            int i = 0;
            TestData testData = new TestData();
            testData.Level = ConvertValue<int>(row[i++]);
            testData.Exp = ConvertValue<int>(row[i++]);
            testData.Skills = ConvertList<int>(row[i++]);
            testData.Speed = ConvertValue<float>(row[i++]);
            testData.Name = ConvertValue<string>(row[i++]);

            loader.tests.Add(testData);
        }

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }*/
    #endregion

    #region Helpers
    private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
    {
        Loader loader = new Loader();
        FieldInfo field = loader.GetType().GetFields()[0];
        field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

        string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
        AssetDatabase.Refresh();
    }

    private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
    {
        List<LoaderData> loaderDatas = new List<LoaderData>();

        string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

        for (int l = 1; l < lines.Length; l++)
        {
            string[] row = lines[l].Replace("\r", "").Split(',');
            if (row.Length == 0)
                continue;
            if (string.IsNullOrEmpty(row[0]))
                continue;

            LoaderData loaderData = new LoaderData();
            var fields = GetFieldsInBase(typeof(LoaderData));

            for (int f = 0; f < fields.Count; f++)
            {
                FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
                Type type = field.FieldType;

                if (field.HasAttribute(typeof(NonSerializedAttribute)))
                    continue;

                if (type.IsGenericType)
                {
                    object value = ConvertList(row[f], type);
                    field.SetValue(loaderData, value);
                }
                else
                {
                    object value = ConvertValue(row[f], field.FieldType);
                    field.SetValue(loaderData, value);
                }
            }

            loaderDatas.Add(loaderData);
        }

        return loaderDatas;
    }

    private static object ConvertValue(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.ConvertFromString(value);
    }

    private static object ConvertList(string value, Type type)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        // Reflection
        Type valueType = type.GetGenericArguments()[0];
        Type genericListType = typeof(List<>).MakeGenericType(valueType);
        var genericList = Activator.CreateInstance(genericListType) as IList;

        // Parse Excel
        var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

        foreach (var item in list)
            genericList.Add(item);

        return genericList;
    }

    public static List<FieldInfo> GetFieldsInBase(Type type, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        HashSet<string> fieldNames = new HashSet<string>(); // �ߺ�����
        Stack<Type> stack = new Stack<Type>();

        while (type != typeof(object))
        {
            stack.Push(type);
            type = type.BaseType;
        }

        while (stack.Count > 0)
        {
            Type currentType = stack.Pop();

            foreach (var field in currentType.GetFields(bindingFlags))
            {
                if (fieldNames.Add(field.Name))
                {
                    fields.Add(field);
                }
            }
        }

        return fields;
    }
    #endregion

#endif
}
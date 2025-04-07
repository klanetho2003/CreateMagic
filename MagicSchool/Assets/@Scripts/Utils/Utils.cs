using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public static class Utils
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Color HexToColor(string color)
    {
        if (color.Contains("#") == false)
            color = $"#{color}";

        ColorUtility.TryParseHtmlString(color, out Color parseColor);

        return parseColor;
    }

    public static EObjectType DetermineTargetType(EObjectType ownerType, bool findAllies)
    {
        if (ownerType == EObjectType.Student)
        {
            return findAllies ? EObjectType.Student : EObjectType.Monster;
        }
        if (ownerType == EObjectType.Monster)
        {
            return findAllies ? EObjectType.Monster : EObjectType.Student;
        }

        return EObjectType.None;
    }

    public static float GetEffectRadius(EEffectSize size)
    {
        switch (size)
        {
            case EEffectSize.CircleSmall:
                return EFFECT_SMALL_RADIUS;
            case EEffectSize.CircleNormal:
                return EFFECT_NORMAL_RADIUS;
            case EEffectSize.CircleBig:
                return EFFECT_BIG_RADIUS;
            case EEffectSize.ConeSmall:
                return EFFECT_SMALL_RADIUS * 2f;
            case EEffectSize.ConeNormal:
                return EFFECT_NORMAL_RADIUS * 2f;
            case EEffectSize.ConeBig:
                return EFFECT_BIG_RADIUS *2f;
            default:
                return EFFECT_SMALL_RADIUS;
        }
    }

    public static bool IsIncludedList<T>(List<T> parentList, List<T>childList)
    {
        // 1. 리스트 길이 체크 (빠른 탈출)
        if (childList.Count > parentList.Count)
            return false;

        // 2. 첫 번째 값이 일치하는 경우만 비교 (빠른 필터링)
        if (!EqualityComparer<T>.Default.Equals(parentList[0], childList[0]))
            return false;

        // 3. 비교
        if (parentList.Take(childList.Count).SequenceEqual(childList))
            return true; // 일치하는 리스트 발견 시 반환

        return false;
    }

    public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
    {
        float totalWeight = sequence.Sum(weightSelector);

        double itemWeightIndex = new System.Random().NextDouble() * totalWeight;
        float currentWeightIndex = 0;

        foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
        {
            currentWeightIndex += item.Weight;

            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if (currentWeightIndex >= itemWeightIndex)
                return item.Value;

        }

        return default(T);
    }

    //Math Util
    public static Vector2 GenerateMonsterSpawnPosition(Vector2 characterPosition, float minDistance = 10.0f, float maxDistance = 20.0f)
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float distance = Random.Range(minDistance, maxDistance);

        float xDist = Mathf.Cos(angle) * distance;
        float yDist = Mathf.Sin(angle) * distance;

        Vector2 spawnPosition = characterPosition + new Vector2(xDist, yDist);

        return spawnPosition;
    }

    public static Vector2 ApplyPositionWeight(float weightX, float weightY, Vector2 lookDir)
    {
        if (lookDir == Vector2.zero)
            return Vector2.zero;

        Vector2 input = new Vector2(weightX, weightY);
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, lookDir.normalized);
        return rotation * input;
    }
}

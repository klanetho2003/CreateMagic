using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

public class Utils
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

    public static ECreatureType DetermineTargetType(ECreatureType ownerType, bool findAllies)
    {
        if (ownerType == ECreatureType.Student)
        {
            return findAllies ? ECreatureType.Student : ECreatureType.Monster;
        }
        if (ownerType == ECreatureType.Monster)
        {
            return findAllies ? ECreatureType.Monster : ECreatureType.Student;
        }

        return ECreatureType.None;
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
}

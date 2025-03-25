using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Utils.FindChild(go, name = null, recursive = false);
    }

    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Utils.FindChild<T>(go, name = null, recursive = false);
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static bool IsValid(this BaseController bc, bool ignoreDead = false, bool ignoreSpawning = false)
    {
        if (bc == null || bc.isActiveAndEnabled == false)
            return false;

        CreatureController creature = bc as CreatureController;
        if (creature != null)
        {
            // 사망 상태면
            if (ignoreDead == false && creature.CreatureState == Define.CreatureState.Dead)
                return false;
            // 스폰 중인 상태면
            if (ignoreSpawning == false && creature.CreatureState == Define.CreatureState.Spawning) 
                return false;
        }

        return true;
    }

    public static void MakeMask(this ref LayerMask mask, List<Define.ELayer> list)
    {
        foreach (Define.ELayer layer in list)
            mask |= (1 << (int)layer);
    }

    public static void AddLayer(this ref LayerMask mask, Define.ELayer layer)
    {
        mask |= (1 << (int)layer);
    }

    public static void RemoveLayer(this ref LayerMask mask, Define.ELayer layer)
    {
        mask &= ~(1 << (int)layer);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Utils.GetOrAddComponent<T>(go);
    }

    public static GameObject FindChild(this GameObject go, string name = null, bool recursive = false)
    {
        return Utils.FindChild(go, name = null, recursive = false);
    }

    public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        return Utils.FindChild<T>(go, name = null, recursive = false);
    }

    public static void OnBurnEx(this CreatureController cc, CreatureController owner, int addDuration)
    {
        if (cc.IsValid() == false)
            return;

        EffectedCreature effectedCC = cc.gameObject.GetComponent<EffectedCreature>();
        if (effectedCC == null)
            return;

        effectedCC.OnBurn(owner, addDuration);
    }



    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    public static bool IsValid(this BaseController bc)
    {
        return bc != null && bc.isActiveAndEnabled;
    }
}

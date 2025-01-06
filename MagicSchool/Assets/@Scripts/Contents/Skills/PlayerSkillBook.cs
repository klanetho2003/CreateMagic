using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.Input.OnKeyHandler += KeyTest;

        return true;
    }

    void KeyTest(Define.KeyEvent key)
    {
        Debug.Log($"{key.ToString()}    ,   {gameObject.name}");
    }
}

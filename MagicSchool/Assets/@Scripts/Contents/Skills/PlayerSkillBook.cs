using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    string _skillKey;
    SingleSkill _skill;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        

        return true;
    }

    //  To Do : Input되었을 때 근접 공격 함수

    public void BuildSKillKey(string inputKey)
    {
        _skillKey = _skillKey + inputKey;

        Debug.Log($"SkillKey -> {_skillKey}");

        if (inputKey != "A" && inputKey != "S" && inputKey != "D")
            return;
        CastSkill();
    }

    public void CastSkill()
    {
        SingleSkillDict.TryGetValue(_skillKey, out _skill);
        _skill.ActivateSkill();

        _skillKey = "";
    }
}

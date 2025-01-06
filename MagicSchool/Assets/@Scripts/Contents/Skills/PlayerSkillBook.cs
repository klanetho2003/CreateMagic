using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBook : BaseSkillBook
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        

        return true;
    }

    // To Do : void BuildSkill() -> input된 값 모델링 -> 만약 Input된 값이 A, S, D 중 하나면 모델링 최종본을 변수에 저장
    // To Do : void override?? 스킬사용() -> dictionary.TrygetValue를 사용해서 모델링 최종본과 매칭되는 skill이 있는지 확인 -> 있으면 사용
}

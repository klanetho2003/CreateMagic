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

    // To Do : void BuildSkill() -> input�� �� �𵨸� -> ���� Input�� ���� A, S, D �� �ϳ��� �𵨸� �������� ������ ����
    // To Do : void override?? ��ų���() -> dictionary.TrygetValue�� ����ؼ� �𵨸� �������� ��Ī�Ǵ� skill�� �ִ��� Ȯ�� -> ������ ���
}

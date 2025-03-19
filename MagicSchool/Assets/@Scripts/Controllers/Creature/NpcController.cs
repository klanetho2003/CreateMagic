using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class NpcController : BaseController
{
    public NpcData Data { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Npc;
        return true;
    }

    public void SetInfo(int dataId)
    {
        Data = Managers.Data.NpcDic[dataId];
        gameObject.name = $"{Data.DataId}_{Data.Name}";

        #region Animation
        SetAnimation(Data.AnimatorDataID, Data.SortingLayerName, SortingLayers.NPC);
        // Anim.Play(NpcData.AnimName, -1, 0f); // Animation은 자동으로 틀어질 것
        #endregion

        // Npc 상호작용을 위한 버튼
        UI_World_NpcInteraction button = Managers.UI.MakeWorldSpaceUI<UI_World_NpcInteraction>(gameObject.transform);
        button.transform.localPosition = new Vector3(0f, 3f);
        button.SetInfo(DataTemplateID, this);
    }
}

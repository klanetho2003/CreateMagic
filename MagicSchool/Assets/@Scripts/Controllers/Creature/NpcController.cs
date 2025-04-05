using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface INpcInteraction
{
    public void SetInfo(NpcController owner);
    public void HandleOnClickEvent();
    public bool CanInteract(); // ex 거리 멀어지면 Interraction 불가
}

public class NpcController : BaseController
{
    public NpcData Data { get; set; }

    public ENpcType NpcType { get { return Data.NpcType; } }

    public INpcInteraction Interaction { get; private set; }

    private UI_World_NpcInteraction _ui;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Npc;
        return true;
    }

    public override void UpdateController()
    {
        base.UpdateController();

        if (Interaction != null && Interaction.CanInteract())
        {
            _ui.gameObject.SetActive(true);
        }
        else
        {
            _ui.gameObject.SetActive(false);
        }
    }

    public void SetInfo(int dataId)
    {
        Data = Managers.Data.NpcDic[dataId];
        gameObject.name = $"{Data.DataId}_{Data.Name}";

        // Animation
        SetAnimation(Data.AnimatorDataID, Data.SortingLayerName, SortingLayers.NPC);
        // Anim.Play(NpcData.AnimName, -1, 0f); // Animation은 자동으로 틀어질 것

        // Npc 상호작용을 위한 버튼
        _ui = Managers.UI.MakeWorldSpaceUI<UI_World_NpcInteraction>(gameObject.transform);
        _ui.transform.localPosition = new Vector3(0f, 3f);
        _ui.SetInfo(DataTemplateID, this);

        // Interaction Setting
        switch (Data.NpcType)
        {
            case ENpcType.Waypoint:
                Interaction = new WayPointInteraction();
                break;
            case ENpcType.Quest:
                Interaction = new QuestInteraction();
                break;
        }

        Interaction?.SetInfo(this);
    }

    public virtual void OnClickEvent()
    {
        Interaction?.HandleOnClickEvent();
    }
}

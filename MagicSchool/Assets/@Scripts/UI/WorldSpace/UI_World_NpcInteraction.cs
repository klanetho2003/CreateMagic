using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_World_NpcInteraction : UI_Base
{
    private NpcController _owner;

    enum Buttons
    {
        InteractionButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));

        return true;
    }

    public void SetInfo(int dataId, NpcController owner)
    {
        _owner = owner;
        GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);
    }

    private void OnClickInteractionButton(PointerEventData evt)
    {
        _owner?.OnClickEvent();

        Debug.Log("OnClickInteractionButton");
    }
}

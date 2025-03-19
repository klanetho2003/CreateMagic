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
        switch (_owner.Data.NpcType)
        {
            case Define.ENpcType.Camp:
                break;
            case Define.ENpcType.Portal:
                break;
            case Define.ENpcType.Waypoint:
                Managers.UI.ShowPopupUI<UI_WaypointPopup>();
                break;
            case Define.ENpcType.BlackSmith:
                break;
            case Define.ENpcType.Guild:
                break;
            case Define.ENpcType.TreasureBox:
                break;
            case Define.ENpcType.Dungeon:
                break;
            default:
				break;
		}

        // Debug.Log("OnClickInteractionButton");
    }
}

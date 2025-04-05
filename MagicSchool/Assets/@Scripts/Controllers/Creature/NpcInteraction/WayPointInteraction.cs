using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointInteraction : INpcInteraction
{
    private NpcController _owner;

    public void SetInfo(NpcController owner)
    {
        _owner = owner;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void HandleOnClickEvent()
    {
        // Temp
        Managers.UI.ShowPopupUI<UI_WaypointPopup>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_World_HpBar : UI_Base
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

        /*BindButtons(typeof(Buttons));

        GetComponent<Canvas>().worldCamera = Camera.main;*/

        return true;
    }

    public void SetInfo(int dataId, NpcController owner)
    {
        _owner = owner;
        // GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);
    }
}

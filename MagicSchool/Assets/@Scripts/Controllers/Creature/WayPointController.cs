using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointController : BaseController
{
    enum Transforms
    {
        WayPoint_Up,
        WayPoint_Right,
        WayPoint_Down,
        WayPoint_Left,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        string[] wayPoints = Enum.GetNames(typeof(Transforms));

        foreach (string name in wayPoints)
        {
            Managers.Game.WayPoints.Add(Utils.FindChild<Transform>(gameObject, name));
        }

        return true;
    }
}

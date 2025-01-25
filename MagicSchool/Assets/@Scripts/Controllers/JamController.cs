using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class JamController : BaseController
{
    public override bool Init()
    {
        base.Init();

        ObjectType = EObjectType.Env;

        return true;
    }
}

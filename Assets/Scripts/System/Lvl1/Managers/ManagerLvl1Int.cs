using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLvl1Int : MissionsSystemManager<int>
{
    private void Awake()
    {
        lvl = 1;
        base.Awake();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGH_NPCIdle : INPCState
{
    private WGH_NPCController controller;
    public WGH_NPCIdle(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter() { }

    public void OnUpdate() { }

    public void Exit() { }
}

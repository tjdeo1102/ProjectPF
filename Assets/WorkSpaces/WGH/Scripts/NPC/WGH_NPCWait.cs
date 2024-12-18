using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCWait : INPCState
{
    private WGH_NPCController controller;
    public WGH_NPCWait(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        Debug.Log("Wait ป๓ลย");
    }

    public void OnUpdate()
    {
        
    }

    public void Exit() { }
}

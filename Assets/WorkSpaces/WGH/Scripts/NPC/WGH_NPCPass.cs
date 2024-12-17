using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGH_NPCPass : INPCState
{
    private ENpcType npcType;
    private WGH_NPCController controller;
    public WGH_NPCPass(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter() 
    {
        Debug.Log("pass»óÅÂ");
    }

    public void OnUpdate() 
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            controller.ChangeState(new WGH_NPCEnter(controller, controller.Agent));
        }
    }

    public void Exit() { }
}

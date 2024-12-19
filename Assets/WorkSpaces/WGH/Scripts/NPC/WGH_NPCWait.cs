using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public enum E_ReactUiType
{
    BEST,
    GOOD,
    BAD
}
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
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            controller.SelectReactUINetwork((int)E_ReactUiType.BEST);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controller.SelectReactUINetwork((int)E_ReactUiType.GOOD);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controller.SelectReactUINetwork((int)E_ReactUiType.BAD);
        }
    }

    public void Exit() 
    {
        
    }
}

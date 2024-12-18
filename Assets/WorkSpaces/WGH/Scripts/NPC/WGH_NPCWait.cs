using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCWait : INPCState
{
    private WGH_NPCController controller;
    private int smellCount = 0;
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
            controller.Best.gameObject.SetActive(true);
            controller.Good.gameObject.SetActive(false);
            controller.Bad.gameObject.SetActive(false);
            smellCount++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controller.Best.gameObject.SetActive(false);
            controller.Good.gameObject.SetActive(true);
            controller.Bad.gameObject.SetActive(false);
            smellCount++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controller.Best.gameObject.SetActive(false);
            controller.Good.gameObject.SetActive(false);
            controller.Bad.gameObject.SetActive(true);
            smellCount++;
        }

        if(smellCount >= 3)
        {
            controller.ChangeState(new WGH_NPCPurchase(controller), E_NpcType.PURCHASE);
        }
    }

    public void Exit() 
    {
        controller.Best.gameObject.SetActive(false);
        controller.Good.gameObject.SetActive(false);
        controller.Bad.gameObject.SetActive(false);
    }
}

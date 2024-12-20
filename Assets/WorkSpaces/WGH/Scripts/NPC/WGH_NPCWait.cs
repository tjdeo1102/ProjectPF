using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public enum E_ReactUiType
{
    BEST,
    LIKE,
    QUESTION,
    DESPAIR
}

public enum E_BottleType
{
    a,
    b,
    c,
    d,
    e
}

public class WGH_NPCWait : INPCState
{
    private WGH_NPCController controller;
    private int randomNum;
    public WGH_NPCWait(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        randomNum = Random.Range(0, 5);
        if (PhotonNetwork.IsMasterClient)
        {
            controller.SelectBottleUINetwork(randomNum);
        }
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
            controller.SelectReactUINetwork((int)E_ReactUiType.LIKE);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controller.SelectReactUINetwork((int)E_ReactUiType.QUESTION);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
        }
    }

    public void Exit() 
    {
        
    }
}

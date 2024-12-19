using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCGoToCounter : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    public WGH_NPCGoToCounter(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter() 
    {
        Debug.Log("go counter ป๓ลย");
        agent.SetDestination(controller.Counter);
    }

    public void OnUpdate() 
    {
        if(agent.remainingDistance < agent.stoppingDistance && agent.pathPending == false)
        {
            //controller.ChangeState(new WGH_NPCWait(controller), E_NpcType.WAIT);
            controller.ChangeStateNetwork((int)E_StateType.WAIT);
        }
    }

    public void Exit() { }
}

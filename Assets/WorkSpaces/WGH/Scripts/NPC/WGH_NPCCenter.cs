using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCCenter : INPCState
{
    private WGH_NPCController controller;
    private NavMeshAgent agent;

    public WGH_NPCCenter(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        //Debug.Log("Center ป๓ลย");
        WGH_NPCCreator.Instance.isCounter = true;
        agent.SetDestination(controller.StoreCenter);
    }

    public void OnUpdate()
    {
        if(agent.remainingDistance < agent.stoppingDistance && agent.pathPending == false && WGH_NPCCreator.Instance.isCounter == false)
        {
            controller.ChangeStateNetwork((int)E_StateType.COUNTER);
        }
    }

    public void Exit() { }
}

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
        agent.SetDestination(controller.StoreCenter);
    }

    public void OnUpdate()
    {
        if(agent.remainingDistance < agent.stoppingDistance && WGH_NPCCreator.Instance.isCounter == false)
        {
            controller.ChangeStateNetwork((int)E_StateType.COUNTER);
        }
        else if(agent.remainingDistance < agent.stoppingDistance && WGH_NPCCreator.Instance.isCounter == true)
        {
            controller.ChangeStateNetwork((int)E_StateType.EXIT);
        }
    }

    public void Exit() { }
}

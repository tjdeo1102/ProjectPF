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
        agent.SetDestination(controller.Counter);
    }

    public void OnUpdate() 
    {

    }

    public void Exit() { }
}

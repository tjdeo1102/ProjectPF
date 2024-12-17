using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCExit : INPCState
{
    private E_NpcType npcType;
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    public WGH_NPCExit(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter() 
    {
        agent.SetDestination(controller.PassPos);
    }

    public void OnUpdate() { }

    public void Exit() { }
}

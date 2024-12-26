using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCExit : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    public WGH_NPCExit(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter() 
    {
        agent.isStopped = false;
        Debug.Log("exit ป๓ลย");
        agent.SetDestination(controller.PassPos);
        WGH_NPCCreator.Instance.isCounter = false;
        WGH_NPCCreator.Instance.isExplore = false;
    }

    public void OnUpdate() 
    {
        if(agent.remainingDistance < agent.stoppingDistance && agent.pathPending == false)
        {
            PhotonNetwork.Destroy(agent.gameObject);
        }
    }

    public void Exit() { }
}

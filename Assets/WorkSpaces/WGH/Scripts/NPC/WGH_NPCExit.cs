using Photon.Pun;
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
        agent.SetDestination(controller.PassPos);
    }

    public void OnUpdate()
    {
        if (Vector3.Distance(controller.transform.position, controller.PassPos) < 1.5f)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                PhotonNetwork.Destroy(controller.gameObject);
            }
        }
    }
    
    public void Exit() { }
}

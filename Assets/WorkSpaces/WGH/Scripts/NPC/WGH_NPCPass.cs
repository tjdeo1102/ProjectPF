using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCPass : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    private int randNum;
    public WGH_NPCPass(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        controller.anim.SetTrigger("Walk");
        if((WGH_NPCCreator.Instance.isExplore == false || WGH_NPCCreator.Instance.isCounter == false) && controller.isOnlyPassNpc == false)
        {
            controller.ChangeStateNetwork((int)E_StateType.ENTER);
        }
        else
        {
            agent.SetDestination(controller.PassPos);
        }
    }

    public void OnUpdate()
    {
        if(PhotonNetwork.IsMasterClient && agent.remainingDistance < 3)
        {
            PhotonNetwork.Destroy(agent.gameObject);
        }
    }

    public void Exit() { }
}

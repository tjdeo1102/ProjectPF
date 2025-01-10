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
        controller.SetAnimNetwork("Walk");
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

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
        randNum = Random.Range(1, controller.PassDenominatorNum + 1);
        Debug.Log("pass ป๓ลย");
        
        if((WGH_NPCCreator.Instance.isExplore == false || WGH_NPCCreator.Instance.isCounter == false) && randNum <= controller.PassNumeratorNum)
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
        if(Vector3.Distance(controller.transform.position, controller.PassPos) < 0.1f)
        {
            PhotonNetwork.Destroy(agent.gameObject);
        }
    }

    public void Exit() { }
}

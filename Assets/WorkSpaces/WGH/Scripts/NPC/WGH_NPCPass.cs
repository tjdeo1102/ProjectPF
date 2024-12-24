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
        Debug.Log("pass 상태");
        // TODO : Enter상태가 되는 조건 추가
        if(WGH_NPCCreator.Instance.isEntered == false && randNum <= controller.PassNumeratorNum)
        {
            WGH_NPCCreator.Instance.isEntered = true;
            controller.ChangeStateNetwork((int)E_StateType.ENTER);
        }
        else
        {
            agent.SetDestination(controller.PassPos);
        }
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controller.ChangeStateNetwork((int)E_StateType.ENTER);
        }
        if(Vector3.Distance(controller.transform.position, controller.PassPos) < 0.1f)
        {
            PhotonNetwork.Destroy(agent.gameObject);
        }
    }

    public void Exit() { }
}

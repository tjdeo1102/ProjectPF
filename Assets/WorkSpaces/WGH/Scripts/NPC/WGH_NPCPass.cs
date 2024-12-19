using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCPass : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;
    public WGH_NPCPass(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        Debug.Log("pass 상태");
        // TODO : Enter상태가 되는 조건 추가
        agent.SetDestination(controller.PassPos);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controller.ChangeStateNetwork((int)E_StateType.ENTER);
        }
    }

    public void Exit() { }
}

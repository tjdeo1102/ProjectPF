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
        Debug.Log("pass ����");
        // TODO : Enter���°� �Ǵ� ���� �߰�
        agent.SetDestination(controller.PassPos);
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //controller.ChangeState(new WGH_NPCEnter(controller, controller.Agent), E_NpcType.ENTER);
            controller.ChangeStateNetwork((int)E_NpcType.ENTER);
        }
    }

    public void Exit() { }
}

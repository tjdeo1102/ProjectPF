using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCEnter : INPCState
{
    private E_NpcType npcType;
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    private int randomNum;
    public WGH_NPCEnter(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        Debug.Log("Enter상태 진입");
        agent.SetDestination(controller.Entrance);
        randomNum = Random.Range(1, 3);
    }

    public void OnUpdate()
    {
        if (agent.remainingDistance < agent.stoppingDistance && agent.pathPending == false)
        {
            switch (randomNum)
            {
                case 1:
                    controller.ChangeState(new WGH_NPCGoToCounter(controller, controller.Agent), E_NpcType.COUNTER);
                    break;

                case 2:
                    controller.ChangeState(new WGH_NPCExplore(controller, controller.Agent), E_NpcType.EXPLORE);
                    break;
            }
            
        }
    }

    public void Exit()
    {
        Debug.Log("Enter상태 탈출");
    }
}

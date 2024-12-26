using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCEnter : INPCState
{
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
        WGH_NPCCreator.Instance.isExplore = true;
        Debug.Log("Enter 상태 진입");
        agent.SetDestination(controller.Entrance);
        randomNum = Random.Range(1, 3);
    }

    public void OnUpdate()
    {
        //TODO : 조건에 맞춰서 수정
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            switch (randomNum)
            {
                case 1:
                    controller.ChangeStateNetwork((int)E_StateType.COUNTER);
                    break;

                case 2:
                    controller.ChangeStateNetwork((int)E_StateType.EXPLORE);
                    break;
            }
            
        }
    }

    public void Exit()
    {
        Debug.Log("Enter상태 탈출");
    }
}

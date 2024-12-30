using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCExplore : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;
    private int randomNum;
    public WGH_NPCExplore(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        Debug.Log("Explore ป๓ลย");
        randomNum = Random.Range(1, controller.ExploreDenominatorNum + 1);
        WGH_NPCCreator.Instance.isExplore = true;
    }

    public void OnUpdate()
    {
        if (agent.pathPending == false && controller.isExplore == false)
        {
            if (randomNum <= controller.ExploreNumeratorNum && WGH_NPCCreator.Instance.isCounter == false)
            {
                controller.ChangeStateNetwork((int)E_StateType.CENTER);
            }
            else
            {
                controller.ChangeStateNetwork((int)E_StateType.EXIT);
            }
        }
    }

    public void Exit() 
    {
        WGH_NPCCreator.Instance.isExplore = false;
    }
}

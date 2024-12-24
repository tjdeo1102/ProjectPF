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
    }

    public void OnUpdate()
    {
        if (Vector3.Distance(controller.gameObject.transform.position, controller.ExplorePos2) < 0.1f && agent.pathPending == false)
        {
            if (randomNum <= controller.ExploreNumeratorNum)
            {

                controller.ChangeStateNetwork((int)E_StateType.COUNTER);
            }
            else
            {
                controller.ChangeStateNetwork((int)E_StateType.EXIT);
            }
        }
    }

    public void Exit() { }
}

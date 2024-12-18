using System.Collections;
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
        Debug.Log("Explore»óÅÂ");
        randomNum = Random.Range(1, 3);
    }

    public void OnUpdate()
    {
        if(Vector3.Distance(controller.gameObject.transform.position, controller.ExplorePos2) < 0.1f && agent.pathPending == false)
        {
            switch(randomNum)
            {
                case 1:
                    controller.ChangeState(new WGH_NPCGoToCounter(controller, controller.Agent), E_NpcType.COUNTER);
                    break;
                case 2:
                    controller.ChangeState(new WGH_NPCExit(controller, controller.Agent), E_NpcType.EXIT);
                    break;
            }
        }
    }

    public void Exit() { }

    
}

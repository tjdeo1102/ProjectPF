using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCEnter : MonoBehaviour
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    public WGH_NPCEnter(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter() { }

    public void OnUpdate() { }

    public void Exit() { }
}

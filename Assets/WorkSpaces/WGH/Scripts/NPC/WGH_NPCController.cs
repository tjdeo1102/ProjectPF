using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCController : MonoBehaviour
{
    [SerializeField] private INPCState curState;

    private WGH_NPCIdle idleState;

    private void Awake()
    {
        idleState = new WGH_NPCIdle(this);
    }

    private void Start()
    {
        ChangeState(idleState);
    }

    private void Update()
    {
        curState.OnUpdate();
    }

    public void ChangeState(INPCState newState)
    {
        curState?.Exit();
        curState = newState;
        curState.Enter();
    }
}

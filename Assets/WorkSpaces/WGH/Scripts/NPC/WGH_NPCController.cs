using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum E_NpcType
{
    NONE,
    PASS,
    ENTER,
    EXPLORE,
    COUNTER,
    PURCHASE,
    EXIT,
    ENnpcType_MAX
}

public class WGH_NPCController : MonoBehaviour
{
    [SerializeField] private E_NpcType npcType;
    [SerializeField] private INPCState curState;

    private WGH_NPCPass passState;
    private WGH_NPCEnter enterState;
    private WGH_NPCExplore exploreState;
    private WGH_NPCGoToCounter goToCouterState;
    private WGH_NPCExit exitState;

    private NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField] private Vector3 entrance;                                     // �Ա� Vector
    public Vector3 Entrance { get { return entrance; } }

    [SerializeField] private Vector3 passPos;                                      // pass ��Ʈ Vector
    public Vector3 PassPos { get { return passPos; } }

    [SerializeField] private Vector3 explorePos1;                                  // explore ��ġ 1
    public Vector3 ExplorePos1 { get {  return explorePos1; } }

    [SerializeField] private Vector3 explorePos2;                                  // explore ��ġ 2
    public Vector3 ExplorePos2 { get { return explorePos2; } }

    [SerializeField] private Vector3 counter;                                      // counter ��ġ
    public Vector3 Counter { get { return counter; } }

    private void Awake()
    {
        npcType = E_NpcType.NONE;

        agent = GetComponent<NavMeshAgent>();
        passState = new WGH_NPCPass(this, agent);
        enterState = new WGH_NPCEnter(this, agent);
        exploreState = new WGH_NPCExplore(this, agent);
        goToCouterState = new WGH_NPCGoToCounter(this, agent);
        exitState = new WGH_NPCExit(this, agent);
    }

    private void Start()
    {
        ChangeState(passState, E_NpcType.PASS);
    }

    private void Update()
    {
        curState?.OnUpdate();
    }

    public void ChangeState(INPCState newState, E_NpcType type)
    {
        curState?.Exit();
        curState = newState;
        npcType = type;
        curState.Enter();
        // explore ���¿� ������ ��쿡�� �����ϴ� �ڷ�ƾ
        if(npcType == E_NpcType.EXPLORE)
        {
            StartCoroutine(ExploreRoutine());
        }
    }

    IEnumerator ExploreRoutine()
    {
        agent.SetDestination(explorePos1);

        while (true)
        {
            yield return new WaitForSeconds(1);
            if (Vector3.Distance(gameObject.transform.position, explorePos1) < agent.stoppingDistance)
            {
                agent.SetDestination(explorePos2);
                yield break;
            }
        }
    }
}

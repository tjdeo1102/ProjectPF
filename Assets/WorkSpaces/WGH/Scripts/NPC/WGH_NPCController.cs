using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum E_NpcType
{
    NONE,
    PASS,                                                                         // 가게 밖 이동 상태
    ENTER,                                                                        // 가게 입장
    EXPLORE,                                                                      // 가게 둘러보기
    COUNTER,                                                                      // 카운터로 가는 상태
    WAIT,                                                                         // 카운터에서 대기 + 시향 + 리액션
    PURCHASE,                                                                     // 시향 후 병 타입 제시 + 수령할때까지 대기 상태
    EXIT,                                                                         // 퇴장
    ENnpcType_MAX
}

public class WGH_NPCController : MonoBehaviourPun
{
    [SerializeField] private E_NpcType npcType;
    [SerializeField] private INPCState curState;

    private WGH_NPCPass passState;
    private WGH_NPCEnter enterState;
    private WGH_NPCExplore exploreState;
    private WGH_NPCGoToCounter goToCouterState;
    private WGH_NPCExit exitState;
    private WGH_NPCWait wait;
    private WGH_NPCPurchase purchase;

    private NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField] private Vector3 entrance;                                      // 입구 Vector
    public Vector3 Entrance { get { return entrance; } }

    [SerializeField] private Vector3 passPos;                                       // pass 루트 Vector
    public Vector3 PassPos { get { return passPos; } }

    [SerializeField] private Vector3 explorePos1;                                   // explore 위치 1
    public Vector3 ExplorePos1 { get { return explorePos1; } }

    [SerializeField] private Vector3 explorePos2;                                   // explore 위치 2
    public Vector3 ExplorePos2 { get { return explorePos2; } }

    [SerializeField] private Vector3 counter;                                       // counter 위치
    public Vector3 Counter { get { return counter; } }

    [SerializeField] private Collider interactionArea;                              // 시향 콜라이더
    public Collider InteractionArea { get { return interactionArea; } }

    [SerializeField] private Image best;                                            // Best 이미지
    public Image Best { get { return best; } }

    [SerializeField] private Image good;                                            // Good 이미지
    public Image Good { get { return good; } }

    [SerializeField] private Image bad;                                             // Bad 이미지
    public Image Bad { get { return bad; } }

    private void Awake()
    {
        npcType = E_NpcType.NONE;

        agent = GetComponent<NavMeshAgent>();
        interactionArea = GetComponentInChildren<CapsuleCollider>();
        best = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        good = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        bad = transform.GetChild(0).GetChild(2).GetComponent<Image>();

        passState = new WGH_NPCPass(this, agent);
        enterState = new WGH_NPCEnter(this, agent);
        exploreState = new WGH_NPCExplore(this, agent);
        goToCouterState = new WGH_NPCGoToCounter(this, agent);
        wait = new WGH_NPCWait(this);
        purchase = new WGH_NPCPurchase(this);
        exitState = new WGH_NPCExit(this, agent);
    }

    private void Start()
    {
        ChangeStateNetwork((int)E_NpcType.PASS);
    }

    private void Update()
    {
        curState?.OnUpdate();
    }

    /// <summary>
    /// RPC 함수(npc 상태 동기화)
    /// </summary>
    [PunRPC]
    public void ChangeState(E_NpcType type)
    {
        INPCState newState = FindStateType((int)type);
        curState?.Exit();
        curState = newState;
        npcType = type;
        curState.Enter();
        // explore 상태에 진입할 경우에만 진행하는 코루틴
        if (npcType == E_NpcType.EXPLORE)
        {
            StartCoroutine(ExploreRoutine());
        }
    }

    /// <summary>
    /// RPC 함수 호출
    /// </summary>
    public void ChangeStateNetwork(int type)
    {
        photonView.RPC("ChangeState", RpcTarget.AllBuffered, type);
    }

    /// <summary>
    /// int 값을 대입하면 그에 맞는 상태 클래스를 찾아주는 함수
    /// </summary>
    public INPCState FindStateType(int type)
    {
        switch (type)
        {
            case 1:
                return new WGH_NPCPass(this, Agent);
            case 2:
                return new WGH_NPCEnter(this, Agent);
            case 3:
                return new WGH_NPCExplore(this, Agent);
            case 4:
                return new WGH_NPCGoToCounter(this, Agent);
            case 5:
                return new WGH_NPCWait(this);
            case 6:
                return new WGH_NPCPurchase(this);
            case 7:
                return new WGH_NPCExit(this, Agent);
            default:
                return null;
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

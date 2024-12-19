using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum E_StateType
{
    NONE,
    PASS,                                                                         // 가게 밖 이동 상태
    ENTER,                                                                        // 가게 입장
    EXPLORE,                                                                      // 가게 둘러보기
    COUNTER,                                                                      // 카운터로 가는 상태
    WAIT,                                                                         // 카운터에서 대기 + 시향 + 리액션 + 병 타입 제시
    PURCHASE,                                                                     // 수령할때까지 대기 상태
    EXIT,                                                                         // 퇴장
    ENnpcType_MAX
}

public class WGH_NPCController : MonoBehaviourPun
{
    [SerializeField, Tooltip("현재 상태")] private E_StateType stateType;
    private INPCState curState;

    private WGH_NPCPass passState;
    private WGH_NPCEnter enterState;
    private WGH_NPCExplore exploreState;
    private WGH_NPCGoToCounter goToCouterState;
    private WGH_NPCExit exitState;
    private WGH_NPCWait wait;
    private WGH_NPCPurchase purchase;

    private NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField, Tooltip("입구 위치")] private Vector3 entrance;                                    // 입구 Vector
    public Vector3 Entrance { get { return entrance; } }

    [SerializeField, Tooltip("가게를 지나칠때의 도착 위치")] private Vector3 passPos;                     // pass 루트 Vector
    public Vector3 PassPos { get { return passPos; } }

    [SerializeField, Tooltip("가게 내부 탐색위치 1")] private Vector3 explorePos1;                       // explore 위치 1

    [SerializeField, Tooltip("가게 내부 탐색위치 2")] private Vector3 explorePos2;                       // explore 위치 2
    public Vector3 ExplorePos2 { get { return explorePos2; } }

    [SerializeField, Tooltip("카운터 위치")] private Vector3 counter;                                    // counter 위치
    public Vector3 Counter { get { return counter; } }

    [SerializeField, Tooltip("시향 콜라이더")] private Collider interactionArea;                         // 시향 콜라이더

    public Collider InteractionArea { get { return interactionArea; } }

    private Image best;                                                                                 // Best 이미지

    private Image good;                                                                                 // Good 이미지

    private Image bad;                                                                                  // Bad 이미지

    [Tooltip("병 UI 목록")] public Sprite[] bottleUI;

    [Tooltip("병 UI")] public Image purchaseUI;

    private void Awake()
    {
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
        ChangeStateNetwork((int)E_StateType.PASS);
    }

    private void Update()
    {
        curState?.OnUpdate();
    }

    /// <summary>
    /// RPC 함수(npc 상태 동기화)
    /// </summary>
    [PunRPC]
    public void ChangeState(E_StateType type)
    {
        INPCState newState = FindStateType((int)type);
        curState?.Exit();
        curState = newState;
        stateType = type;
        curState.Enter();
        // explore 상태에 진입할 경우에만 진행하는 코루틴
        if (stateType == E_StateType.EXPLORE)
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

    /// <summary>
    /// 리액션 UI 활성/비활성화 함수
    /// </summary>
    [PunRPC]
    public void SelectReactUI(int uiType)
    {
        switch (uiType)
        {
            case 0:
                StartCoroutine(FloatBestRoutine());
                break;
            case 1:
                StartCoroutine(FloatGoodRoutine());
                break;
            case 2:
                StartCoroutine(FloatBadRoutine());
                break;
        }
    }

    public void SelectReactUINetwork(int uiType)
    {
        photonView.RPC("SelectReactUI", RpcTarget.All, uiType);
    }

    [PunRPC]
    public void SelectBottleUI(int bottleType)
    {
        purchaseUI.sprite = bottleUI[bottleType];
    }

    public void SelectBottleUINetwork(int bottleType)
    {

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

    IEnumerator FloatBestRoutine()
    {
        best.gameObject.SetActive(true);
        good.gameObject.SetActive(false);
        bad.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        best.gameObject.SetActive(false);
        ChangeStateNetwork((int)E_StateType.PURCHASE);
        yield break;
    }

    IEnumerator FloatGoodRoutine()
    {
        best.gameObject.SetActive(false);
        good.gameObject.SetActive(true);
        bad.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        good.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator FloatBadRoutine()
    {
        best.gameObject.SetActive(false);
        good.gameObject.SetActive(false);
        bad.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        bad.gameObject.SetActive(false);
        yield break;
    }
}

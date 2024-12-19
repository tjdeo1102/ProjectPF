using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum E_StateType
{
    NONE,
    PASS,                                                                         // ���� �� �̵� ����
    ENTER,                                                                        // ���� ����
    EXPLORE,                                                                      // ���� �ѷ�����
    COUNTER,                                                                      // ī���ͷ� ���� ����
    WAIT,                                                                         // ī���Ϳ��� ��� + ���� + ���׼� + �� Ÿ�� ����
    PURCHASE,                                                                     // �����Ҷ����� ��� ����
    EXIT,                                                                         // ����
    ENnpcType_MAX
}

public class WGH_NPCController : MonoBehaviourPun
{
    [SerializeField, Tooltip("���� ����")] private E_StateType stateType;
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

    [SerializeField, Tooltip("�Ա� ��ġ")] private Vector3 entrance;                                    // �Ա� Vector
    public Vector3 Entrance { get { return entrance; } }

    [SerializeField, Tooltip("���Ը� ����ĥ���� ���� ��ġ")] private Vector3 passPos;                     // pass ��Ʈ Vector
    public Vector3 PassPos { get { return passPos; } }

    [SerializeField, Tooltip("���� ���� Ž����ġ 1")] private Vector3 explorePos1;                       // explore ��ġ 1

    [SerializeField, Tooltip("���� ���� Ž����ġ 2")] private Vector3 explorePos2;                       // explore ��ġ 2
    public Vector3 ExplorePos2 { get { return explorePos2; } }

    [SerializeField, Tooltip("ī���� ��ġ")] private Vector3 counter;                                    // counter ��ġ
    public Vector3 Counter { get { return counter; } }

    [SerializeField, Tooltip("���� �ݶ��̴�")] private Collider interactionArea;                         // ���� �ݶ��̴�

    public Collider InteractionArea { get { return interactionArea; } }

    private Image best;                                                                                 // Best �̹���

    private Image good;                                                                                 // Good �̹���

    private Image bad;                                                                                  // Bad �̹���

    [Tooltip("�� UI ���")] public Sprite[] bottleUI;

    [Tooltip("�� UI")] public Image purchaseUI;

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
    /// RPC �Լ�(npc ���� ����ȭ)
    /// </summary>
    [PunRPC]
    public void ChangeState(E_StateType type)
    {
        INPCState newState = FindStateType((int)type);
        curState?.Exit();
        curState = newState;
        stateType = type;
        curState.Enter();
        // explore ���¿� ������ ��쿡�� �����ϴ� �ڷ�ƾ
        if (stateType == E_StateType.EXPLORE)
        {
            StartCoroutine(ExploreRoutine());
        }
    }

    /// <summary>
    /// RPC �Լ� ȣ��
    /// </summary>
    public void ChangeStateNetwork(int type)
    {
        photonView.RPC("ChangeState", RpcTarget.AllBuffered, type);
    }

    /// <summary>
    /// int ���� �����ϸ� �׿� �´� ���� Ŭ������ ã���ִ� �Լ�
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
    /// ���׼� UI Ȱ��/��Ȱ��ȭ �Լ�
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

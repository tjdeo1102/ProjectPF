using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum E_NpcType
{
    NONE,
    PASS,                                                                         // ���� �� �̵� ����
    ENTER,                                                                        // ���� ����
    EXPLORE,                                                                      // ���� �ѷ�����
    COUNTER,                                                                      // ī���ͷ� ���� ����
    WAIT,                                                                         // ī���Ϳ��� ��� + ���� + ���׼�
    PURCHASE,                                                                     // ���� �� �� Ÿ�� ���� + �����Ҷ����� ��� ����
    EXIT,                                                                         // ����
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

    [SerializeField] private Vector3 entrance;                                      // �Ա� Vector
    public Vector3 Entrance { get { return entrance; } }

    [SerializeField] private Vector3 passPos;                                       // pass ��Ʈ Vector
    public Vector3 PassPos { get { return passPos; } }

    [SerializeField] private Vector3 explorePos1;                                   // explore ��ġ 1
    public Vector3 ExplorePos1 { get { return explorePos1; } }

    [SerializeField] private Vector3 explorePos2;                                   // explore ��ġ 2
    public Vector3 ExplorePos2 { get { return explorePos2; } }

    [SerializeField] private Vector3 counter;                                       // counter ��ġ
    public Vector3 Counter { get { return counter; } }

    [SerializeField] private Collider interactionArea;                              // ���� �ݶ��̴�
    public Collider InteractionArea { get { return interactionArea; } }

    [SerializeField] private Image best;                                            // Best �̹���
    public Image Best { get { return best; } }

    [SerializeField] private Image good;                                            // Good �̹���
    public Image Good { get { return good; } }

    [SerializeField] private Image bad;                                             // Bad �̹���
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
    /// RPC �Լ�(npc ���� ����ȭ)
    /// </summary>
    [PunRPC]
    public void ChangeState(E_NpcType type)
    {
        INPCState newState = FindStateType((int)type);
        curState?.Exit();
        curState = newState;
        npcType = type;
        curState.Enter();
        // explore ���¿� ������ ��쿡�� �����ϴ� �ڷ�ƾ
        if (npcType == E_NpcType.EXPLORE)
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

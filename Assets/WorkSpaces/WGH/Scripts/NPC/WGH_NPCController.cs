using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum E_StateType
{
    NONE,
    PASS,                                                                         // 가게 밖 이동 상태
    ENTER,                                                                        // 가게 입장
    EXPLORE,                                                                      // 가게 둘러보기
    CENTER,                                                                       // 가게 중앙으로 이동
    COUNTER,                                                                      // 카운터로 가는 상태
    WAIT,                                                                         // 카운터에서 대기 + 시향 + 리액션 + 병 타입 제시
    EXIT,                                                                         // 퇴장
    ENnpcType_MAX
}

public class WGH_NPCController : MonoBehaviourPun
{
    [Header("상태")]
    [Tooltip("현재 상태")] public E_StateType stateType;
    public INPCState curState;
    private WGH_NPCPass passState;
    private WGH_NPCEnter enterState;
    private WGH_NPCExplore exploreState;
    private WGH_NPCCenter center;
    private WGH_NPCGoToCounter goToCouterState;
    private WGH_NPCWait wait;
    private WGH_NPCExit exitState;

    private NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public NavMeshAgent Agent { get { return agent; } }
    [HideInInspector] public WGH_SmellStick SmellStick;
    [HideInInspector] public WGH_NPCPause pause;
    [HideInInspector] public Renderer SkinnedMeshRenderer;
    public GameObject TestNote;

    [Header("선호도")]
    private WGH_NPCNote npcNote;
    public E_WGH_PerfumeType PerfumeType;
    public E_BottleType BottleType;
    public E_WGH_NoteType BestMaterial;
    public E_WGH_NoteType LikeMaterial;
    public E_WGH_NoteType LikeMaterial2;
    public E_WGH_NoteType QuestionMaterial;
    public E_WGH_NoteType QuestionMaterial2;

    [Header("탐색하고 난 뒤 행동에 대한 난수")]
    [SerializeField] int exploreDenominatorNum;
    public int ExploreDenominatorNum { get { return exploreDenominatorNum; } }
    [SerializeField] int exploreNumeratorNum;
    public int ExploreNumeratorNum { get { return exploreNumeratorNum; } }

    [HideInInspector] public Vector3 PassPos;                                                           // pass 루트 Vector
    [HideInInspector] public Vector3 Entrance;
    [HideInInspector] public Vector3 ExplorePos1;                                                       // explore 위치 1
    [HideInInspector] public Vector3 ExplorePos2;                                                       // explore 위치 2
    [HideInInspector] public Vector3 StoreCenter;
    [HideInInspector] public Vector3 Counter;

    [Header("NPC 상호작용 콜라이더")]
    [SerializeField, Tooltip("시향 콜라이더")] private Collider interactionArea;                         // 시향 콜라이더
    public Collider InteractionArea { get { return interactionArea; } }
    
    [Header("표정 텍스쳐")]
    public Texture[] faceTextures;

    [Header("리액션 이펙트")]
    [SerializeField] private ParticleSystem bestEmotion;
    [SerializeField] private ParticleSystem likeEmotion;
    [SerializeField] private ParticleSystem questionEmotion;
    [SerializeField] private ParticleSystem despairEmotion;
    public ParticleSystem SuccessEmotion;
    public ParticleSystem FailEmotion;

    [Header("UI")]
    [Tooltip("병 UI 목록")] public Sprite[] BottleUis;
    [Tooltip("병 UI")] public Image BottleUI;

    private Coroutine exploreRoutine;
    public bool isExplore;
    public bool isOnlyPassNpc;                  // pass만 하는 npc인지 여부
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        BottleUI = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        passState = new WGH_NPCPass(this, agent);
        enterState = new WGH_NPCEnter(this, agent);
        exploreState = new WGH_NPCExplore(this, agent);
        goToCouterState = new WGH_NPCGoToCounter(this, agent);
        wait = new WGH_NPCWait(this, agent);
        center = new WGH_NPCCenter(this, agent);
        exitState = new WGH_NPCExit(this, agent);
    }

    private void Start()
    {
        if(transform.GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            SkinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        }
        // Pause 이벤트 등록
        pause = GameObject.FindGameObjectWithTag("Pause").GetComponentInChildren<WGH_NPCPause>();
        pause.OnPause.AddListener(StartPauseBehaviour);

        if (PhotonNetwork.IsMasterClient)
            ChangeStateNetwork((int)E_StateType.PASS);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            curState?.OnUpdate();
    }

    /// <summary>
    /// Pause 상태 전환 시 Exit 상태로 변경
    /// </summary>
    public void StartPauseBehaviour()
    {
        if(WGH_NPCCreator.Instance.isPause == true)
        {
            ChangeStateNetwork((int)E_StateType.EXIT);
        }
    }

    /// <summary>
    /// RPC 함수(npc 상태 동기화)
    /// </summary>
    [PunRPC]
    public void ChangeState(E_StateType type)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        INPCState newState = FindStateType((int)type);
        curState?.Exit();
        curState = newState;
        stateType = type;
        curState.Enter();
        // explore 상태에 진입할 경우에만 진행하는 코루틴
        if (stateType == E_StateType.EXPLORE)
        {
            exploreRoutine = StartCoroutine(ExploreRoutine());
            isExplore = true;
        }
        else if ((stateType == E_StateType.COUNTER || stateType == E_StateType.EXIT) && isExplore == true)
        {
            StopCoroutine(exploreRoutine);
            isExplore = false;
        }
    }

    /// <summary>
    /// RPC 함수 호출
    /// </summary>
    public void ChangeStateNetwork(int type)
    {
        photonView.RPC("ChangeState", RpcTarget.All, type);
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
                return new WGH_NPCCenter(this, Agent);
            case 5:
                return new WGH_NPCGoToCounter(this, Agent);
            case 6:
                return new WGH_NPCWait(this, Agent);
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
                StartCoroutine(FloatBestEmotionRoutine());
                break;
            case 1:
                StartCoroutine(FloatLikeEmotionRoutine());
                break;
            case 2:
                StartCoroutine(FloatQuestionEmotionRoutine());
                break;
            case 3:
                StartCoroutine(FloatDespairEmotionRoutine());
                break;
            case 4:
                SuccessEmotion.Play();
                break;
            case 5:
                FailEmotion.Play();
                break;
        }
    }

    public void SelectReactUINetwork(int uiType)
    {
        photonView.RPC("SelectReactUI", RpcTarget.All, uiType);
    }

    [PunRPC]
    public void SelectOrderUI(int bottleType, int perfumeType)
    {
        if (BottleUI.gameObject.activeSelf == false)
        {
            BottleUI.gameObject.SetActive(true);
        }

        BottleUI.sprite = BottleUis[bottleType];
        BottleType = (E_BottleType)bottleType;
    }

    public void SelectOrderUINetwork(int bottleType, int perfumeType)
    {
        photonView.RPC("SelectOrderUI", RpcTarget.All, bottleType, perfumeType);
    }

    IEnumerator CheckDist(Vector3 pos)
    {
        while (true) 
        {
            if(agent.remainingDistance < 0.2f && agent.pathPending == false)
            {
                SetAnimNetwork("Explore");
                Debug.Log("Explore");
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator ExploreRoutine()
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        int randomCount = Random.Range(1, 4);
        int randomSec = Random.Range(3, 11);
        int randomSec2 = Random.Range(3, 11);
        int randomSec3 = Random.Range(3, 11);

        bool exploreLeft = false;
        for (int i = 1; i <= randomCount; i++)
        {
            if (!exploreLeft)
            {
                exploreLeft = true;
                agent.SetDestination(ExplorePos2);
                StartCoroutine(CheckDist(ExplorePos2));
                yield return new WaitForSeconds(randomSec + 5);
                
            }
            else
            {
                exploreLeft = false;
                agent.SetDestination(ExplorePos1);
                StartCoroutine(CheckDist(ExplorePos1));
                yield return new WaitForSeconds(randomSec2 + 5);
            }
            SetAnimNetwork("Walk");
            Debug.Log("Walk");
        }
        isExplore = false;
        yield break;
    }

    public void SetAnimNetwork(string name)
    {
        photonView.RPC("SetAnimRPC", RpcTarget.All, name);
    }

    [PunRPC]
    private void SetAnimRPC(string name)
    {
        anim.SetTrigger(name);
    }
    IEnumerator FloatBestEmotionRoutine()
    {
        SetAnimNetwork("Best");
        SetEmotion(4);
        bestEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        SetEmotion(0);
        bestEmotion.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator FloatLikeEmotionRoutine()
    {
        SetAnimNetwork("Yes");
        SetEmotion(3);
        likeEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        SetEmotion(0);
        likeEmotion.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator FloatQuestionEmotionRoutine()
    {
        SetAnimNetwork("Question");
        SetEmotion(2);
        questionEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        SetEmotion(0);
        questionEmotion.gameObject.SetActive(false);
        yield break;
    }

    IEnumerator FloatDespairEmotionRoutine()
    {
        SetAnimNetwork("No");
        SetEmotion(1);
        despairEmotion.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        SetEmotion(0);
        despairEmotion.gameObject.SetActive(false);
        yield break;
    }

    public void SetEmotion(int num)
    {
        photonView.RPC("SetEmotionRPC", RpcTarget.All, num);
    }

    [PunRPC]
    public void SetEmotionRPC(int num)
    {
        SkinnedMeshRenderer.materials[1].mainTexture = faceTextures[num];
    }
}

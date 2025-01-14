using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WGH_NPCCreator : MonoBehaviour
{
    public static WGH_NPCCreator Instance;
    [Header("방문 손님 생성 시간")]
    [SerializeField] private float storeNpcSpawnTime;
    [SerializeField] private float storeNpcCurTime;
    [Header("길거리 손님 생성 시간")]
    [SerializeField] private float passNpcSpawnTime;
    [SerializeField] private float passNpcCurTime;
    [Header("포지션")]
    [SerializeField] private Transform spawnPassLeftPos;
    [SerializeField] private Transform spawnLeftPos;
    [SerializeField] private Transform spawnPassRightPos;
    [SerializeField] private Transform spawnRightPos;
    [SerializeField] private Transform enterancePos;
    [SerializeField] private Transform leftExplorePos;
    [SerializeField] private Transform rightExplorePos;
    [SerializeField] private Transform storeCenterPos;
    [SerializeField] private Transform CounterPos;
    [SerializeField] private Transform passExitPos;   

    [SerializeField] private int stageLevel;

    private bool isPassLeftSpawn;
    private bool isLeftSpawn;
    public bool isCounter;
    public bool isExplore;
    public bool isCheat;

    public bool isPause;                    // 일시정지용
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        //onchangestageinfo 이벤트 구독해서 스테이지가 바뀔때마다 함수 호출
    }
    private void Start()
    {
        // 스테이지 레벨
        stageLevel = KSD_GameManager.Instance.CurrentStageInfo.StageLevel;
        KSD_GameManager.Instance.OnChangeStageInfo.AddListener(StageChange);
    }

    public void StageChange()
    {
        Debug.Log("스테이지 체인지 이벤트 호출");
        stageLevel = KSD_GameManager.Instance.CurrentStageInfo.StageLevel;
        storeNpcSpawnTime = SetCustomerAmount(stageLevel);
    }

    private float SetCustomerAmount(int stageLevel)
    {
        switch(stageLevel)
        {
            case 0:
                return 10;
            case 1:
                return 10;
            case 2:
                return 30;
            case 3:
                return 20;
            case 4:
                return 20;
            case 5:
                return 20;
            case 6:
                return 20;
            case 7:
                return 20;
            case 8:
                return 15;
            case 9:
                return 15;
            case 10:
                return 15;
            default:
                return 10;
        }
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    isPause = !isPause;
        if (PhotonNetwork.IsMasterClient == false && !isCheat)
            return;
        //if(isPause == false)
        //{
            PassNpcSpawn();
            CountTime();
        //}
    }

    public void PassNpcSpawn()
    {
        passNpcCurTime += Time.deltaTime;
        if(passNpcCurTime >= passNpcSpawnTime && isPassLeftSpawn == false)
        {
            int randNum = Random.Range(1, 9);
            isPassLeftSpawn = true;
            GameObject obj = PhotonNetwork.Instantiate($"Customer{randNum}", spawnPassLeftPos.position, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.isOnlyPassNpc = true;
            controller.PassPos = passExitPos.position;
            passNpcCurTime = 0;
        }
        else if(passNpcCurTime >= passNpcSpawnTime && isPassLeftSpawn == true)
        {
            int randNum = Random.Range(1, 9);
            isPassLeftSpawn = false;
            GameObject obj = PhotonNetwork.Instantiate($"Customer{randNum}", spawnPassRightPos.position, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.isOnlyPassNpc = true;
            controller.PassPos = passExitPos.position;
            passNpcCurTime = 0;
        }
    }
    public void CountTime()
    {
        storeNpcCurTime += Time.deltaTime;
        if (storeNpcCurTime >= storeNpcSpawnTime && isLeftSpawn == false)
        {
            int randNum = Random.Range(1, 9);
            isLeftSpawn = true;
            GameObject obj = PhotonNetwork.Instantiate($"Customer{randNum}", spawnLeftPos.position, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.PassPos = spawnRightPos.position;
            controller.Entrance = enterancePos.position;
            controller.ExplorePos1 = leftExplorePos.position;
            controller.ExplorePos2 = rightExplorePos.position;
            controller.StoreCenter = storeCenterPos.position;
            controller.Counter = CounterPos.position;
            storeNpcCurTime = 0f;
        }
        else if(storeNpcCurTime >= storeNpcSpawnTime && isLeftSpawn == true)
        {
            int randNum = Random.Range(1, 9);
            isLeftSpawn = false;
            GameObject obj = PhotonNetwork.Instantiate($"Customer{randNum}", spawnRightPos.position, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.PassPos = spawnLeftPos.position;
            controller.Entrance = enterancePos.position;
            controller.ExplorePos1 = leftExplorePos.position;
            controller.ExplorePos2 = rightExplorePos.position;
            controller.StoreCenter = storeCenterPos.position;
            controller.Counter = CounterPos.position;
            storeNpcCurTime = 0f;
        }
    }

    /// <summary>
    /// 치트키 함수 : 바로 카운터로 손님을 오게 만드는 치트키
    /// </summary>
    public void OnCheat()
    {
        StartCoroutine(CheatRoutine());
    }
    /// <summary>
    /// 치트키 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CheatRoutine()
    {
        int randNum = Random.Range(1, 9);
        isCheat = true;
        GameObject obj = PhotonNetwork.Instantiate($"Customer{randNum}", spawnLeftPos.position, Quaternion.identity);
        WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
        controller.PassPos = new Vector3(-spawnRightPos.position.x, spawnRightPos.position.y, spawnRightPos.position.z);
        controller.Entrance = enterancePos.position;
        controller.ExplorePos1 = leftExplorePos.position;
        controller.ExplorePos2 = rightExplorePos.position;
        controller.StoreCenter = storeCenterPos.position;
        controller.Counter = CounterPos.position;
        yield return new WaitForSeconds(0.1f);
        controller.ChangeStateNetwork((int)E_StateType.COUNTER);
        isCheat = false;
    }
}

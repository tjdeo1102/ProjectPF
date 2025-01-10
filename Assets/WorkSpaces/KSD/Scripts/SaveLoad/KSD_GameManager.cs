using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class KSD_GameManager : MonoBehaviourPun
{
    [Header("싱글톤")]
    public static KSD_GameManager Instance;

    [Header("기본 설정")]
    [SerializeField] public int maxCustomerCount;
    [SerializeField] private int currentStageID;
    [SerializeField] private int returnSceneIndex;

    [Header("네트워크 안정화")]
    [SerializeField] private float networkDelay;

    [Header("플레이어 스폰 설정")]
    [SerializeField] private string playerPrefabPath;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float randomSpawnLength;

    [Header("스테이지 별 게임 데이터")]
    [SerializeField] KSD_StageGameData[] gameDatas;
    public KSD_StageGameData currentGameData;

    private GameObject player;

    [Header("게임 매니저 구성 요소")]
    [SerializeField] KSD_EnvironmentManager environmentManager;

    [Header("현재 스테이지 정보 설정 및 갱신")]
    public KSD_StageInfo CurrentStageInfo;
    public UnityEvent OnChangeStageInfo;                            // 스테이지 정보가 바뀔 때 호출할 이벤트
    public UnityEvent OnExitStage;                                  // 스테이지가 종료되었을 때(손님 카운트가 다 채워졌을 때), 호출할 이벤트

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 해당 씬에서만 존재할 싱글톤이므로, 씬전환시 삭제가능하도록 구현
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnable()
    {
        OnChangeStageInfo.AddListener(UpdateGameData);
    }

    public void OnDisable()
    {
        OnChangeStageInfo.RemoveListener(UpdateGameData);
    }

    void Start()
    {
        // 각 클라이언트의 맵 로드 대기
        StartCoroutine(NetworkInit());
    }

    private IEnumerator NetworkInit()
    {
        yield return new WaitForSeconds(networkDelay);
        InitStage();
    }

    /// <summary>
    /// 클라이언트의 초기 세팅
    /// </summary>
    public void InitStage()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentMapData", out object currentMapData))
        {
            CurrentStageInfo = JsonUtility.FromJson<KSD_StageInfo>((string)currentMapData);
            UpdateEnvironment();
            InitPlayer();
            if (gameDatas.Length >= CurrentStageInfo.StageLevel)
            {
                currentGameData = gameDatas[CurrentStageInfo.StageLevel-1];
            }
            OnChangeStageInfo?.Invoke();
            Debug.Log("맵 데이터가 로드되었습니다.");
        }
        else
        {
            Debug.Log("맵 데이터가 로드 실패");
        }
    }

    private void InitPlayer()
    {
        var spawnPos = new Vector3(Random.Range(-randomSpawnLength, randomSpawnLength) + spawnPosition.position.x,
                                   spawnPosition.position.y,
                                   Random.Range(-randomSpawnLength, randomSpawnLength) + spawnPosition.position.z);
        player = PhotonNetwork.Instantiate(playerPrefabPath, spawnPos, Quaternion.identity);
    }

    [PunRPC]
    private void AddFinishPlayerCountRPC(int addCount)
    {

        CurrentStageInfo.FinishPlayerCount += addCount;

        if (CurrentStageInfo.FinishPlayerCount >= maxCustomerCount)
        {
            // 스테이지 상승
            CurrentStageInfo.StageLevel++;
            CurrentStageInfo.FinishPlayerCount = CurrentStageInfo.FinishPlayerCount - maxCustomerCount;
            // 스테이지 종료 관련 이벤트 호출
            OnExitStage?.Invoke();
        }
        if (gameDatas.Length >= CurrentStageInfo.StageLevel)
        {
            currentGameData = gameDatas[CurrentStageInfo.StageLevel - 1];
        }
        OnChangeStageInfo?.Invoke();

        UpdateEnvironment();
    }

    /// <summary>
    /// 현재 완료된 플레이어의 카운트를 변경하는 함수
    /// </summary>
    /// <param name="addCount"> 추가되거나 감소될 카운트 </param>
    public void AddFinishPlayerCount(int addCount)
    {
        if (PhotonNetwork.IsMasterClient) photonView.RPC("AddFinishPlayerCountRPC", RpcTarget.All, addCount);
    }


    public void SaveAndQuitGame()
    {
        PhotonNetwork.LeaveRoom();
        KSD_SaveLoad.Instance.SaveToDatabase(PhotonNetwork.LocalPlayer.NickName, CurrentStageInfo).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("맵을 저장하는 데 문제가 발생했습니다. 로비로 복귀합니다.");
                PhotonNetwork.LoadLevel(returnSceneIndex);
            }
            else
            {
                if (task.Result == true)
                {
                    Debug.Log("정상적으로 맵 저장 성공, 로비로 복귀합니다.");
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
                else
                {
                    Debug.LogError("맵을 저장하는 데 문제가 발생했습니다. 로비로 복귀합니다.");
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
            }
        });
    }

    public void DontSaveQuitGame()
    {
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} 나감");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(returnSceneIndex);
    }

    public void UpdateEnvironment()
    {
        if (environmentManager != null)
        {
            environmentManager.ChangeLight(CurrentStageInfo.FinishPlayerCount, maxCustomerCount);
        }
    }

    public void UpdateGameData()
    {
        maxCustomerCount = currentGameData.TargetNPCCount;
    }
}

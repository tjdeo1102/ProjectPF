using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KSD_CheatManager : MonoBehaviourPunCallbacks
{
    public enum TeleportSpot
    {
        PerfumeMaterial, Dispensor, Counter
    }
    public static KSD_CheatManager Instance;

    [Header("손님 입장 버튼 (바로 카운터로 향하도록) ")]
    [SerializeField] private WGH_NPCCreator npcManager;

    public void GoCounter()
    {
        npcManager.OnCheat();
    }

    [Header("작업 위치 순간이동")]
    public List<Transform> SpotList;

    public void Teleport(Transform teleportObject, TeleportSpot teleportSpot)
    {
        if (teleportObject != null && SpotList != null)
        {
            var newPos = SpotList[(int)teleportSpot].position;
            teleportObject.position = new Vector3(newPos.x, teleportObject.position.y, newPos.z);
        }
    }

    // 3. 돈 무한 (현재 적용 중)
    
    [Header("무한 모드")]
    [SerializeField] private KSD_GameManager gameManager;
    public void AddDay()
    {
        gameManager.AddFinishPlayerCount(gameManager.maxCustomerCount);
    }

    public void AddFinishNPC()
    {
        gameManager.AddFinishPlayerCount(1);
    }


    [Header("더미 캐릭터 (협동 요소를 싱글로 하기)")]
    [SerializeField] private KSD_CauldronController cauldronManager;
    public bool IsAlwaysFire
    {
        get { return cauldronManager.AlwaysFire; }
        set { cauldronManager.AlwaysFire = value; }
    }
    public bool IsAlwaysShake
    {
        get { return cauldronManager.AlwaysShake; }
        set { cauldronManager.AlwaysShake = value; }
    }

    [Header("게임 초기화 기능")]
    [SerializeField] private int gameSceneNum;
    public void ReturnLobby()
    {
        gameManager.DontSaveQuitGame();
    }

    public void ReturnGame()
    {
        PhotonNetwork.LoadLevel(gameSceneNum);
        // 씬만 리로드 하면, 게임 매니저 재생성에 의해 맵 초기화
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}

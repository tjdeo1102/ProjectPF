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
    
    [Header("손님 및 상점 설정")]
    [SerializeField] private KSD_GameManager gameManager;
    [SerializeField] private LSY_ItemManager itemManager;
    public void AddDay()
    {
        gameManager.AddFinishPlayerCount(gameManager.maxCustomerCount);
    }

    public void AddFinishNPC()
    {
        gameManager.AddFinishPlayerCount(1);
    }

    public void AddHundredMoney()
    {
        gameManager.CurrentStageInfo.StageMoney += 100;
        itemManager.UpdateMoney();
    }


    [Header("더미 캐릭터 (협동 요소를 싱글로 하기)")]
    [SerializeField] private KSD_CauldronController cauldronManager;
    [SerializeField] private PTK_HandleElec handleElecManager;
    [SerializeField] private LSY_Elevator liftManager;
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

    public bool IsLiftBucket
    {
        get { return liftManager.isLiftUp; }
        set { liftManager.isLiftUp = value; }
    }

    public void AlwaysElecToggle()
    {
        if (handleElecManager.isCheatModeActive) handleElecManager.CheatModeOff();
        else handleElecManager.CheatModeOn();
    }

    public void ReturnLobby()
    {
        gameManager.Quit(true, true, false);
    }

    public void ReloadGame()
    {
        gameManager.Quit(false, false, true);
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

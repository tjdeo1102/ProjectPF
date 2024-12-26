using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WGH_NPCCreator : MonoBehaviour
{
    public static WGH_NPCCreator Instance;

    [SerializeField] private float spawnTime;
    [SerializeField] private float curTime;
    [SerializeField] private Vector3 spawnLeftPos;
    [SerializeField] private Vector3 spawnRightPos;

    private bool isLeftSpawn;
    public bool isEntered;
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

    }

    private void Start()
    {
        spawnTime = 10f;                        // 임시 시간 배정
        curTime = 7f;                           // 임시 시간 배정
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        CountTime();
    }

    public void CountTime()
    {
        curTime += Time.deltaTime;
        if (curTime >= spawnTime && isLeftSpawn == false)
        {
            isLeftSpawn = true;
            GameObject obj = PhotonNetwork.Instantiate("Customer", spawnLeftPos, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.PassPos = new Vector3(-spawnLeftPos.x, spawnLeftPos.y, spawnLeftPos.z);
            curTime = 0f;
        }
        else if(curTime >= spawnTime && isLeftSpawn == true)
        {
            isLeftSpawn = false;
            GameObject obj = PhotonNetwork.Instantiate("Customer", spawnRightPos, Quaternion.identity);
            WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
            controller.PassPos = new Vector3(-spawnRightPos.x, spawnRightPos.y, spawnRightPos.z);
            curTime = 0f;
        }
    }
}

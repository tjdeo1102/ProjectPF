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
    public bool isCounter;
    public bool isExplore;
    public bool isCheat;
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

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false && !isCheat)
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
        isCheat = true;
        GameObject obj = PhotonNetwork.Instantiate("Customer", spawnLeftPos, Quaternion.identity);
        WGH_NPCController controller = obj.GetComponent<WGH_NPCController>();
        yield return new WaitForSeconds(0.1f);
        controller.ChangeStateNetwork((int)E_StateType.COUNTER);
        isCheat = false;
    }
}

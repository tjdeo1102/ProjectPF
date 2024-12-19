using Photon.Pun;
using System.Collections;
using UnityEngine;

public class WGH_NPCCreator : MonoBehaviour
{
    [SerializeField] private float spawnTime;
    [SerializeField] private float curTime;
    [SerializeField] private Vector3 spawnPos;

    private void Start()
    {
        spawnTime = 60f;                        // 임시 시간 배정
        curTime = 57f;                          // 임시 시간 배정
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
        if (curTime >= spawnTime)
        {
            PhotonNetwork.Instantiate("Customer", spawnPos, Quaternion.identity);
            curTime = 0f;
        }
    }
}

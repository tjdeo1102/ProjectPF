using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KSD_ObjectSpawn : MonoBehaviour
{
    [SerializeField] private float respawnCoolTime = 1f;
    [SerializeField] private string objectPath = "";
    [SerializeField] private Collider spawnArea;

    private string objectName;
    private Coroutine exitObjectRoutine;
    private float spawnDelay = 1f;
    private float timer;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
            
        SpawnObject();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        if (exitObjectRoutine != null)
        {
            timer -= Time.deltaTime;
            if (timer < 0) timer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 방장이 스폰 여부 체크 (서버)
        if (PhotonNetwork.InRoom == false || PhotonNetwork.IsMasterClient == false) return;

        // 태그 비교보다는 이름 비교를 통해 판단 (태그 수 관리)
        if (objectName == other.name && exitObjectRoutine == null)
        {
            // 나간 직후부터 코루틴 실행
            exitObjectRoutine = StartCoroutine(ExitObjectRoutine());
            timer = spawnDelay;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 나가고 일정 시간안에 다시 원래 재료가 들어온 경우에는 코루틴 종료
        if (exitObjectRoutine != null 
            && other.name == objectName
            && timer > 0f)
        {
            StopCoroutine(exitObjectRoutine);
            exitObjectRoutine = null;
        }
    }

    private void SpawnObject()
    {
        // 랜덤 위치 계산
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            spawnArea.transform.position.y,
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        );

        // 재료 생성
        var obj = PhotonNetwork.Instantiate(objectPath, randomPosition, Quaternion.identity);
        objectName = obj.name;
    }

    private IEnumerator ExitObjectRoutine()
    {
        yield return new WaitForSeconds(respawnCoolTime);
        SpawnObject();
        exitObjectRoutine = null;
    }
}

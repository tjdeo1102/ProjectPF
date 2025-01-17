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
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private int objectMaxCount = 3;
    [SerializeField] private bool isMaxSpawn = true;

    private GameObject exitObject;
    private Coroutine exitObjectRoutine;
    private float spawnDelay = 1f;
    private float timer;

    private List<GameObject> objects;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        objects = new List<GameObject>();
        if (isMaxSpawn == true)
        {
            // 최대치만큼 오브젝트 생성
            for (int i = 0; i < objectMaxCount; i++)
            {
                SpawnObject();
            }
        }
        else
        {
            SpawnObject();
        }
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

        if (objects.Contains(other.gameObject) && exitObjectRoutine == null)
        {
            // 나간 직후부터 코루틴 실행
            exitObjectRoutine = StartCoroutine(ExitObjectRoutine());
            exitObject = other.gameObject;
            timer = spawnDelay;

            // 맨 뒤로 추가
            objects.Remove(other.gameObject); 
            objects.Add(other.gameObject);    
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 나가고 일정 시간안에 다시 원래 재료가 들어온 경우에는 코루틴 종료
        if (exitObjectRoutine != null 
            && other.gameObject == exitObject
            && timer > 0f)
        {
            StopCoroutine(exitObjectRoutine);
            exitObjectRoutine = null;
            exitObject = null;
        }
    }

    private void SpawnObject()
    {
        // 재료 생성
        var obj = PhotonNetwork.Instantiate(objectPath, spawnPosition.position, Quaternion.identity);
        objects.Add(obj);
        if (objects.Count > objectMaxCount)
        {
            PhotonNetwork.Destroy(objects[0]);
            objects.RemoveAt(0);
        }
    }

    private IEnumerator ExitObjectRoutine()
    {
        yield return new WaitForSeconds(respawnCoolTime);
        SpawnObject();
        exitObjectRoutine = null;
    }
}

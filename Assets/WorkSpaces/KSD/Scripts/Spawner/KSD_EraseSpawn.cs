using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KSD_EraseSpawn : MonoBehaviour
{
    [SerializeField] private float spawnDistanceX;
    [SerializeField] private float spawnDistanceZ;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private LayerMask spawnLayerMask;          // 해당 레이어만 체크
    [SerializeField] private string objectPath = "";

    private void Start()
    {
        StartCoroutine(CheckAndRespawn());
    }

    private IEnumerator CheckAndRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Collider 가져오기
            Collider collider = GetComponent<Collider>();
            if (collider == null) continue;
            Vector3 colliderSize = collider.bounds.size;

            // 설정된 레이어만 충돌 검사
            Collider[] colliders = Physics.OverlapBox(transform.position, colliderSize / 2, Quaternion.identity, spawnLayerMask);
            HashSet<PerfumeMaterialName> detectedFruits = new HashSet<PerfumeMaterialName>();

            // 콜라이더 내 숯 확인
            if (colliders.Any(a => a.transform.CompareTag("Erase")) == false)
            {
                // 랜덤 위치 계산
                Vector3 randomPosition = new Vector3(
                    Random.Range(-spawnDistanceX + transform.position.x, spawnDistanceX + transform.position.x),
                    transform.position.y,
                    Random.Range(-spawnDistanceZ + transform.position.z, spawnDistanceZ + transform.position.z));

                // 재료 생성
                PhotonNetwork.Instantiate(objectPath, randomPosition, Quaternion.identity);
            }
        }
    }
}

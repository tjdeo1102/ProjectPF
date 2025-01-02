using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_PerfumeMaterialSpawn : MonoBehaviour
{
    [SerializeField] private float spawnDistanceX;
    [SerializeField] private float spawnDistanceZ;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private LayerMask spawnLayerMask;          

    private Dictionary<PerfumeMaterialName, string> objectPath = new Dictionary<PerfumeMaterialName, string>
    {
        { PerfumeMaterialName.Cherry, "PerfumeMaterials/Raw/KSD_Cherry" },
        { PerfumeMaterialName.Blueberry, "PerfumeMaterials/Raw/KSD_Blueberry" },
        { PerfumeMaterialName.Lemon, "PerfumeMaterials/Raw/KSD_Lemon" },
        { PerfumeMaterialName.Grapefruit, "PerfumeMaterials/Raw/KSD_Grapefruit" },
        { PerfumeMaterialName.Cosmos, "PerfumeMaterials/Raw/KSD_Cosmos" },
        { PerfumeMaterialName.Neroli, "PerfumeMaterials/Raw/KSD_Neroli" },
        { PerfumeMaterialName.Galbanum, "PerfumeMaterials/Raw/KSD_Galbanum" },
        { PerfumeMaterialName.TreeBark, "PerfumeMaterials/Raw/KSD_TreeBark" },
        { PerfumeMaterialName.GreenTea, "PerfumeMaterials/Raw/KSD_GreenTea" },
        { PerfumeMaterialName.Rose, "PerfumeMaterials/Raw/KSD_Rose" }
    };

    private Dictionary<PerfumeMaterialName, PerfumeMaterialType> rawInfo = new Dictionary<PerfumeMaterialName, PerfumeMaterialType>
    {
        { PerfumeMaterialName.Cherry, PerfumeMaterialType.Small },
        { PerfumeMaterialName.Blueberry, PerfumeMaterialType.Small },
        { PerfumeMaterialName.Lemon, PerfumeMaterialType.Big },
        { PerfumeMaterialName.Grapefruit, PerfumeMaterialType.Big },
        { PerfumeMaterialName.Cosmos, PerfumeMaterialType.Hub },
        { PerfumeMaterialName.Neroli, PerfumeMaterialType.Hub },
        { PerfumeMaterialName.Galbanum, PerfumeMaterialType.Hub },
        { PerfumeMaterialName.TreeBark, PerfumeMaterialType.Hub },
        { PerfumeMaterialName.GreenTea, PerfumeMaterialType.Hub },
        { PerfumeMaterialName.Rose, PerfumeMaterialType.Hub }
    };

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

            // 콜라이더 내 과일 확인
            foreach (Collider coll in colliders)
            {
                if (coll.TryGetComponent<PTK_Fruit>(out var fruit))
                {
                    if (fruit.fruitInfo.Name != PerfumeMaterialName.Null)
                    {
                        detectedFruits.Add(fruit.fruitInfo.Name);
                    }
                }
            }

            // 누락된 과일 리스폰
            foreach (var materialName in objectPath.Keys)
            {
                if (!detectedFruits.Contains(materialName))
                {
                    // 랜덤 위치 계산
                    Vector3 randomPosition = new Vector3(
                        Random.Range(-spawnDistanceX + transform.position.x, spawnDistanceX + transform.position.x),
                        transform.position.y,
                        Random.Range(-spawnDistanceZ + transform.position.z, spawnDistanceZ + transform.position.z));

                    // InstantiationData 설정
                    object[] instantiationData = new object[]
                    {
                        (byte)materialName,
                        (byte)rawInfo[materialName],
                        (byte)PerfumeMaterialState.Raw
                    };

                    // 재료 생성
                    PhotonNetwork.Instantiate(objectPath[materialName], randomPosition, Quaternion.identity, 0, instantiationData);
                }
            }
        }
    }
}

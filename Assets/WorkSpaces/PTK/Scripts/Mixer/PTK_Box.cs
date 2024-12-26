using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Box : MonoBehaviour
{
    [SerializeField] private PTK_Handle knobEvent;
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnPoint;

    void Start()
    {
        knobEvent.MixDone.AddListener(SpawnObject);
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PTK_Box : MonoBehaviourPun
{
    [SerializeField] private PTK_Handle knobEvent;
    [SerializeField] private Transform spawnPoint;

    void Start()
    {
        knobEvent.mixDone.AddListener(SpawnObject);
    }

    private void SpawnObject()
    {
        photonView.RPC("RPC_SpawnObject", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SpawnObject()
    {
        Vector3 spawnPosition = spawnPoint.position;
        GameObject resultFruit = PhotonNetwork.Instantiate("PTK_Fruit", spawnPosition, Quaternion.identity);
    }
}

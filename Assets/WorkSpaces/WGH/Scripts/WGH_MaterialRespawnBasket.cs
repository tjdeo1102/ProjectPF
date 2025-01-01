using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WGH_MaterialRespawnBasket : MonoBehaviour
{
    [SerializeField] private PerfumeMaterialName materailName;

    [SerializeField] private float respawnTime;
    private float curTime;

    [SerializeField] List<GameObject> list = new List<GameObject>();
    [SerializeField] GameObject material;
    private Stack<GameObject> materials;

    [SerializeField] Transform spawnPos;

    [SerializeField] XRSocketInteractor socket;

    private void Start()
    {
        material = list[(int)materailName];
        socket.startingSelectedInteractable = Resources.Load<GameObject>($"TestMaterial({(int)materailName})").GetComponent<XRBaseInteractable>();
        
        //StartCoroutine(StartSpawnRoutine());
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            socket.enabled = true;
            Spawn();
        }
    }

    public void Spawn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject obj = PhotonNetwork.Instantiate($"TestMaterial({(int)materailName})", spawnPos.position, Quaternion.identity);
        }
    }
    public void OnCatchMaterial()
    {
        socket.enabled = false;
    }

    IEnumerator StartSpawnRoutine()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.InstantiateRoomObject($"TestMaterial({(int)materailName})", spawnPos.position, Quaternion.identity);
        yield break;
    }
}

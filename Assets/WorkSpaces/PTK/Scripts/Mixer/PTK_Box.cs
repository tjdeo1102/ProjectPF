using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PTK_Box : MonoBehaviourPun
{
    //[SerializeField] private PTK_Handle knobEvent;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform returnSpawnPoint;

    [Header("믹서 완료 이벤트")]
    public UnityAction<KSD_PerfumeMaterialInfo> OnMixDone;

    private PTK_Fruit currentFruit;
    private bool isReadyForMix = false;

    void Start()
    {
        //knobEvent.mixDone.AddListener(OnMixDone);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentFruit != null)
        {
            Debug.Log("Already fruit");
            ReturnObject(other.transform);
            return;
        }

        if (!other.CompareTag("Fruit"))
        {
            Debug.Log("Not fruit");
            ReturnObject(other.transform);
            return;
        }

        PTK_Fruit fruit = other.GetComponent<PTK_Fruit>();
        if (fruit.fruitInfo.Type != PerfumeMaterialType.Small)
        {
            Debug.Log($"Fruit type {fruit.fruitInfo.Type}");
            ReturnObject(other.transform);
            return;
        }

        if (fruit.fruitInfo.State == PerfumeMaterialState.Raw)
        {
            currentFruit = fruit;
            isReadyForMix = true;

            //KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Blender_in);

            Debug.Log("Fruit is ready");
        }
        else
        {
            Debug.Log("Not Raw");
            ReturnObject(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentFruit != null && other.transform == currentFruit.transform)
        {
            Debug.Log("Fruit exited");
            currentFruit = null;
            isReadyForMix = false;
        }
    }

    public void MixDone()
    {
        if (isReadyForMix && currentFruit != null)
        {
            currentFruit.SetState(PerfumeMaterialState.Process);

            if (PhotonNetwork.IsMasterClient == true)
            {
                PhotonNetwork.Destroy(currentFruit.gameObject);

                photonView.RPC("RPC_MixDone", RpcTarget.All,
                (byte)currentFruit.fruitInfo.Name,
                (byte)currentFruit.fruitInfo.Type,
                (byte)currentFruit.fruitInfo.State);

                //KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Blender_out);

                isReadyForMix = false;
            }        
        }
        else
        {
            Debug.Log("No fruit");
        }
    }

    [PunRPC]
    private void RPC_MixDone(byte name, byte type, byte state)
    {
        KSD_PerfumeMaterialInfo info = new KSD_PerfumeMaterialInfo();
        info.Name = (PerfumeMaterialName)name;
        info.Type = (PerfumeMaterialType)type;
        info.State = (PerfumeMaterialState)state;
        OnMixDone?.Invoke(info);
    }

    //[PunRPC]
    //private void RPC_SpawnProcessedFruit(byte name, byte type, byte state)
    //{
    //    Vector3 spawnPosition = spawnPoint.position;
    //    Quaternion spawnRotation = Quaternion.identity;

    //    GameObject newFruit = PhotonNetwork.Instantiate(spawnObjectPath, spawnPosition, spawnRotation, data: new object[] {name, type, state});

    //    PTK_Fruit fruitComponent = newFruit.GetComponent<PTK_Fruit>();

    //    if (fruitComponent != null)
    //    {
    //        fruitComponent.fruitInfo = new KSD_PerfumeMaterialInfo
    //        {
    //            Name = (PerfumeMaterialName)name,
    //            Type = (PerfumeMaterialType)type,
    //            State = (PerfumeMaterialState)state
    //        };

    //        Debug.Log("Fruit spawned");
    //    }
    //}

    private void ReturnObject(Transform obj)
    {
        photonView.RPC("RPC_ReturnObject", RpcTarget.All, obj.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void RPC_ReturnObject(int objectViewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(objectViewID);
        Transform objTransform = targetPhotonView.transform;
        objTransform.position = returnSpawnPoint.position;
        objTransform.rotation = returnSpawnPoint.rotation;
    }
}

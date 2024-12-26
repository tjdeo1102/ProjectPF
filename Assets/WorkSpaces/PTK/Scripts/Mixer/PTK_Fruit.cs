using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Fruit : MonoBehaviourPun
{
    KSD_MaterialObject fruit;

    public KSD_PerfumeMaterialInfo data;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Fruit");

            photonView.RPC("RPC_DestroyObjects", RpcTarget.All, collision.gameObject.GetPhotonView().ViewID, photonView.ViewID);
        }
    }

    [PunRPC]
    private void RPC_DestroyObjects(int playerViewID, int fruitViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        if (playerPhotonView != null)
        {
            Destroy(playerPhotonView.gameObject);
        }

        PhotonView fruitPhotonView = PhotonView.Find(fruitViewID);
        if (fruitPhotonView != null)
        {
            Destroy(fruitPhotonView.gameObject);
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class KSD_MaterialObject : MonoBehaviourPun
{
    public KSD_PerfumeMaterialInfo data;

    [PunRPC]
    private void SetInfoRPC(byte name, byte type , byte state)
    {
        data.Name = (PerfumeMaterialName)name;
        data.Type = (PerfumeMaterialType)type;
        data.State = (PerfumeMaterialState)state;
    }

    public void SetInfo(KSD_PerfumeMaterialInfo info)
    {
        photonView.RPC("SetInfoRPC",RpcTarget.All,(byte)info.Name,(byte)info.Type,(byte)info.State);
    }
}

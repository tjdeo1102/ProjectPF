using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Fruit : MonoBehaviourPun
{
    public KSD_PerfumeMaterialInfo fruitInfo;

    private void Awake()
    {
        if (fruitInfo == null)
        {
            fruitInfo = new KSD_PerfumeMaterialInfo
            {
                Name = PerfumeMaterialName.Null,
                Type = PerfumeMaterialType.Null,
                State = PerfumeMaterialState.Raw
            };
        }

        if (photonView.InstantiationData != null)
        {
            fruitInfo.Name = (PerfumeMaterialName)(byte)photonView.InstantiationData[0];
            fruitInfo.Type = (PerfumeMaterialType)(byte)photonView.InstantiationData[1];
            fruitInfo.State = (PerfumeMaterialState)(byte)photonView.InstantiationData[2];
        }
    }

    public void SetState(PerfumeMaterialState newState)
    {
        photonView.RPC("RPC_SetState", RpcTarget.All, (byte)newState);
    }

    [PunRPC]
    private void RPC_SetState(byte newState)
    {
        fruitInfo.State = (PerfumeMaterialState)newState;
    }

}

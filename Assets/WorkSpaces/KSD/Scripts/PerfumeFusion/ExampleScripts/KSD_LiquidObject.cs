using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class KSD_LiquidObject : MonoBehaviourPun
{
    public KSD_PerfumeNoteInfo data;

    [PunRPC]
    private void SetInfoRPC(byte name, byte state)
    {
        data.Name = (PerfumeNoteName)name;
        data.State = (PerfumeNoteState)state;
    }

    public void SetInfo(KSD_PerfumeNoteInfo info)
    {
        photonView.RPC("SetInfoRPC", RpcTarget.All, (byte)info.Name, (byte)info.State);
    }
}

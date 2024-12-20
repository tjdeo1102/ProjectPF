using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LSY_ServerStateLogger : MonoBehaviourPunCallbacks
{
    [SerializeField] ClientState _state;

    private void Update()
    {
        if (_state == PhotonNetwork.NetworkClientState)
            return;

        _state = PhotonNetwork.NetworkClientState;
        Debug.Log($"[PUN] {_state}");
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_TestGameScene : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 5000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions() { IsVisible = false };
        PhotonNetwork.JoinOrCreateRoom("DebugRoom55", options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PlayerSpawn();
    }

    public void PlayerSpawn()
    {
        PhotonNetwork.Instantiate("KSH_Player", Vector3.zero, Quaternion.identity);
    }
}

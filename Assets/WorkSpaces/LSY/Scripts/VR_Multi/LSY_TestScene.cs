using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_TestScene : MonoBehaviourPunCallbacks
{
    public const string lsy_RoomName = "TestRoomlsy";

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions() { IsVisible = false };
        options.MaxPlayers = 8;

        PhotonNetwork.JoinOrCreateRoom(lsy_RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Vector3 randomPos = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3, 3));
        PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
    }
}

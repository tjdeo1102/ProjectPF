using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGH_TestGameScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom";

    [SerializeField] private Vector3 spawnPos;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(100, 1000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions() { IsVisible = false };
        options.MaxPlayers = 2;

        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}


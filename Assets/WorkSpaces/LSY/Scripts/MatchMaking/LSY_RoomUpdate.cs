using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_RoomUpdate : MonoBehaviour
{
    [SerializeField] private LSY_PlayerEntry[] _playerEntries;

    private void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;
        //PhotonNetwork.LocalPlayer.SetReady(false);
        StartCoroutine(WaitForJoinCO());
        //PhotonNetwork.LocalPlayer.SetLoad(false);
        UpdatePlayers();
    }

    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    IEnumerator WaitForJoinCO()
    {
        while (PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null;
        }

        PhotonNetwork.LocalPlayer.SetReady(false);

    }

    public void UpdatePlayers()
    {
        foreach (LSY_PlayerEntry entry in _playerEntries)
        {
            entry.SetEmpty();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPlayerNumber() == -1)
                continue;
            int num = player.GetPlayerNumber();
            _playerEntries[num].SetPlayer(player);
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckAllReady())
        {
            // 게임시작
        }
    }

    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} Enter!");
        UpdatePlayers();
    }

    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} Exit!");
        UpdatePlayers();
    }

    public void UpdatePlayerProperty(Player targetPlayer, ExitGames.Client.Photon.Hashtable properties)
    {
        Debug.Log($"{targetPlayer.NickName} Update!");

        // 레디 커스텀 프로퍼티를 변경한 경우면 READY 키가 있음
        if (properties.ContainsKey(LSY_CustomProperties.READY))
        {
            UpdatePlayers();
        }
    }

    private bool CheckAllReady()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            return false;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false)
                return false;
        }

        return true;
    }

    public void StartGame()
    {
        //PhotonNetwork.LoadLevel("GameScene");
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    //public void LeaveRoom()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}
}

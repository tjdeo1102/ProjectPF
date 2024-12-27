using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_RoomPanel : MonoBehaviour
{
    [SerializeField] LSY_PlayerEntry[] playerEntries;
    [SerializeField] Button startButton;
    [SerializeField] Button closeButton;

    private void OnEnable()
    {
        // PlayerNumbering의 기본값은 -1인데 만약 값이 변하면 방에 들어온 것
        // PlayerNumbering의 값이 변하면 UpadatePlayers함수를 실행시켜서 자신이면 Ready버튼을 활성화 시켜줌
        //                              계속해서 방의 상황을 업데이트 해줌
        PlayerNumbering.OnPlayerNumberingChanged += UpdataPlayers;
        startButton.onClick.AddListener(StartGame);
        closeButton.onClick.AddListener(LeaveRoom);

        // 방에 들어왔을 땐 레디상황을 false로 만들어줌
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
    }

    private void OnDisable()
    {
        // 방에서 나갔을 때
        PlayerNumbering.OnPlayerNumberingChanged -= UpdataPlayers;
    }

    public void UpdataPlayers()
    {
        // 1. 모든 엔트리들을 비어있게 만들어주고 (초기화 느낌)
        foreach (LSY_PlayerEntry entry in playerEntries)
        {
            entry.SetEmpty();
        }

        // 2. 그 후 방에 들어온 플레이어를 업데이트 해줌
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 아직 번호를 할당받지 않았으면 하지 않음
            if (player.GetPlayerNumber() == -1) continue;

            int number = player.GetPlayerNumber();

            // 현재들어온 number의 플레이어를 세팅해줌
            playerEntries[number].SetPlayer(player);
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.interactable = CheckAllReady();
        }
        else
        {
            startButton.interactable = false;
        }
    }

    // 새로운 플레이어가 방에 들어왔을 때
    public void EnterPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 입장!");
        UpdataPlayers();
    }

    // 플레이어가 방에서 나갔을 때
    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 퇴장!");
        UpdataPlayers();
    }

    // 플레이어의 상태가 변경됐을 때 ex) Ready 같은 상황 변경 감지
    public void UpdatePlayerProperty(Player targetPlayer, PhotonHashtable properties)
    {
        UpdataPlayers();
    }

    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 한명이라도 레디상태가 아니라면 false 반환
            if (player.GetReady() == false) return false;
        }

        return true;
    }


    public void StartGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckAllReady())
        {
            //PhotonNetwork.LoadLevel("GameScene");
            Debug.Log("게임시작");
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.Leaving)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}

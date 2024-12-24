using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_LobbySceneCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Main, Lobby ,Room }      // 각 패널을 열거형으로 분류

    // 각 패널 클래스
    [SerializeField] private LSY_RoomPanel roomPanel;
    [SerializeField] private LSY_LobbyPanel lsy_lobbyPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject RoomPanel;

    [SerializeField] TMP_Text roomPlayerNameText;
    [SerializeField] TMP_Text passwordText;
    [SerializeField] TMP_Text playerCountText;

    private void Start()
    {
        // 각 상태에 따라 자동으로 패널 전환
        PhotonNetwork.AutomaticallySyncScene = true;

        // 각 상태에 따라 자동으로 전환할 패널 설정
        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Main);
        }
    }

    // 로그인 성공 시 MenuPanel로 전환
    public override void OnConnectedToMaster()
    {
        Debug.Log("Login Success!");
        PhotonNetwork.JoinLobby();
    }

    // 로그아웃 시 LoginPanel로 전환
    // 로그로 로그아웃 사유 표시
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Logout! (Cause : {cause})");
        SetActivePanel(Panel.Main);
        PhotonNetwork.LeaveLobby();
    }

    // 방생성 성공 로그 출력
    public override void OnCreatedRoom()
    {
        roomPlayerNameText.text = PhotonNetwork.CurrentRoom.Name;
        playerCountText.text = "참가자 "+ PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["IsPasswordProtected"] == true)
        {
            string password = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomPassword"];
            passwordText.text = "비밀번호: " + password;
            Debug.Log("비밀번호 존재하는 방");
        }
        else
        {
            passwordText.text = "";
            Debug.Log("비밀번호 없는 방");
        }
    }



    // 방 생성 실패 실패 사유가 적힌 로그 출력
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed! (Cause : {message}");
    }

    // 방 참여에 성공했을 때 RoomPanel로 전환
    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);

        playerCountText.text = "참가자 " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        roomPlayerNameText.text = PhotonNetwork.CurrentRoom.Name;

        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["IsPasswordProtected"] == true)
        {
            string password = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomPassword"];
            passwordText.text = "비밀번호: " + password;
        }
        else
        {
            passwordText.text = "";
        } 

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        roomPanel.UpdatePlayerProperty(targetPlayer, changedProps);
    }

    // 방 입장 실패 시 실패 사유가 적힌 로그 출력
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Room Enter Failed! (Cause : {message}");

    }

    // 방에서 퇴장 시 MenuPanel로 전환
    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Main);

        if (PhotonNetwork.CurrentRoom != null)
        {
            playerCountText.text = "참가자 " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        }
        else
        {
            playerCountText.text = "참가자 0 / 0"; 
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Join Lobby Success!");
        SetActivePanel(Panel.Main);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Main);
    }

    // 방 목록 업데이트 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방의 목록에 변경이 있는 경우 서버에서 보내는 정보들

        // <주의사항>
        // 1. 처음 로비 입장 시 : 모든 방 목록을 전달
        // 2. 입장 중 방 목록이 변경되는 경우 : 변경된 방 목록만 전달
        lsy_lobbyPanel.UpdateRoomList(roomList);
    }

    // 각 상태에 맞는 패널 전환 기능
    private void SetActivePanel(Panel panel)
    {
        mainPanel.SetActive(panel == Panel.Main);
        lobbyPanel.SetActive(panel == Panel.Lobby);
        RoomPanel.SetActive(panel == Panel.Room);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCountText.text = "참가자 " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerCountText.text = "참가자 " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}

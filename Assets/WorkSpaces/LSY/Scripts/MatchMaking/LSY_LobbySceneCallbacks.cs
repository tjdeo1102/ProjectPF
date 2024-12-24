using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_LobbySceneCallbacks : MonoBehaviourPunCallbacks
{
    public enum Panel { Main, Room }      // 각 패널을 열거형으로 분류

    // 각 패널 클래스
    [SerializeField] private LSY_RoomUpdate roomUpdate;
    [SerializeField] private LSY_LobbyPanel lsy_lobbyPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject RoomPanel;

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
        Debug.Log("<color=yellow>메인방면 연결 콜백</color>");

        Debug.Log("Login Success!");
        //SetActivePanel(Panel.Lobby);
        PhotonNetwork.JoinLobby();
    }

    // 로그아웃 시 LoginPanel로 전환
    // 로그로 로그아웃 사유 표시
    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("<color=yellow>메인방면 연결 해제 콜백</color>");

        Debug.Log($"Logout! (Cause : {cause})");
        SetActivePanel(Panel.Main);
        PhotonNetwork.LeaveLobby();
    }

    // 방생성 성공 로그 출력
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room complete!");
    }

    // 방 생성 실패 실패 사유가 적힌 로그 출력
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Create Room Failed! (Cause : {message}");
    }

    // 방 참여에 성공했을 때 RoomPanel로 전환
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Enter Success!");
        SetActivePanel(Panel.Room);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsPasswordProtected"))
        {
            bool isPasswordProtected = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsPasswordProtected"];
            Debug.Log("Is Password Protected: " + isPasswordProtected);
        }
    }

    // 방에 입장한 플레이어의 프로퍼티를 변경
    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //{
    //    _roomUpdate.UpdatePlayerProperty(targetPlayer, changedProps);
    //}
    // 방 입장 실패 시 실패 사유가 적힌 로그 출력
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Room Enter Failed! (Cause : {message}");

    }

    // 방에서 퇴장 시 MenuPanel로 전환
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room Success!");
        SetActivePanel(Panel.Main);
    }

    // 랜덤매칭 실패 시 실패 사유가 적힌 로그 출력
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Random Match Failed! (Cause : {message}");

        string name = $"Room {Random.Range(1000, 10000)}";                              // 방 이름을 랜덤으로 설정
        RoomOptions options = new RoomOptions() { MaxPlayers = 10 };                     // 방 최대 인원 수를 8로 설정
        PhotonNetwork.CreateRoom(name, options);

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

        RoomPanel.SetActive(panel == Panel.Room);

    }
}

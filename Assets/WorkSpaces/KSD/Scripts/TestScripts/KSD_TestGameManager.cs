using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditor.XR;
using UnityEngine;

public class KSD_TestGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nicknameInputField;
    private const string roomName = "test";
    private bool canJoin = false;
    private void Start()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        canJoin = true;
    }

    public override void OnJoinedRoom()
    {
        print($"�� ���� {PhotonNetwork.CurrentRoom.Name}");

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable mapOwnerProp = new Hashtable();
            mapOwnerProp["MapOwnerName"] = PhotonNetwork.MasterClient.NickName;

            print($"{mapOwnerProp["MapOwnerName"]}�� ���� ��(��������)�� ����");
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapOwnerProp);
        }
    }

    public void JoinRoom()
    {
        if (canJoin)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);

            PhotonNetwork.NickName = nicknameInputField.text;
        }
    }

    public void LeftRoom()
    {
        if (canJoin)
        {
            PhotonNetwork.LeaveRoom();
        }
    }    

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameClosed)
        {
            Debug.Log("�ٸ� �̸��� ������ �ڵ� ���� ������ �� �����ϴ�.");
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            roomOptions.MaxPlayers = 4;
            var newRoomName = roomName + "_" + System.Guid.NewGuid().ToString();
            PhotonNetwork.JoinOrCreateRoom(newRoomName, roomOptions, TypedLobby.Default);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable mapOwnerProp = new Hashtable();
            mapOwnerProp["MapOwnerName"] = PhotonNetwork.MasterClient.NickName;

            print($"{mapOwnerProp["MapOwnerName"]}�� ���� ��(��������)�� ����");
            PhotonNetwork.CurrentRoom.SetCustomProperties(mapOwnerProp);
        }
    }

    public void SceneLoad(int gameSceneNum)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        print("�� �ε�");

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(gameSceneNum);
    }
}
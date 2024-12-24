using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_LobbyPanel : LSY_BaseUI
{
    public const string lsy_RoomName = "LSY_TestRoom";
    public TMP_InputField nickName;

    private void Start()
    {
        isPasswordProtected = false;
        BindAll();
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void CheckGuestNickname()
    {
        string nickname = nickName.text;

        if (nickname == "")
        {
            Debug.LogWarning("닉네임을 입력해주세요");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = nickname;
        Debug.Log($"닉네임: {PhotonNetwork.LocalPlayer.NickName}");
    }

    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject checkImage;            
    private bool isPasswordProtected = false;

    public void CreateRoomMenu()
    {
        GetUI("CreateRoomPanel").SetActive(true);

        GetUI<TMP_InputField>("RoomNameInputField").text = $"{PhotonNetwork.LocalPlayer.NickName}의 신규게임";
    }

    public void CheckImage()
    {
        isPasswordProtected = !isPasswordProtected;

        checkImage.SetActive(isPasswordProtected);

        passwordInputField.gameObject.SetActive(isPasswordProtected);
    }

    public void MakeRoom()
    {
        string roomName = roomNameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("방 이름을 입력하세요.");
            return;
        }

        if (password.Length < 2)
        {
            Debug.Log("비밀번호는 두자리 이상이여야 합니다");
            return;
        }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
        };

        // CustomProperties: 방 내부에 저장되는 속성
        var customProperties = new PhotonHashtable
        {
            { "RoomPassword", isPasswordProtected ? password : string.Empty }, 
            { "IsPasswordProtected", isPasswordProtected } 
        };

        options.CustomRoomProperties = customProperties;

        // CustomRoomPropertiesForLobby: 로비에서 표시할 속성
        options.CustomRoomPropertiesForLobby = new string[] { "IsPasswordProtected", "RoomPassword" }; 

        PhotonNetwork.CreateRoom(roomName, options);
        Debug.Log("방 생성 완료");
        GetUI("CreateRoomPanel").gameObject.SetActive(false);
    }




    [SerializeField] RectTransform roomContent;
    [SerializeField] LSY_RoomEntry roomEntryPrefab;

    private Dictionary<string, LSY_RoomEntry> roomDictionay = new Dictionary<string, LSY_RoomEntry>();

    public void LeaveLobby()
    {
        Debug.Log("로비 퇴장 요청");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomlist)
    {
        foreach (RoomInfo room in roomlist)
        {
            // 방이 사라진 경우, 비공개인 경우, 입장이 불가능한 경우
            if (room.RemovedFromList == true || room.IsVisible == false || room.IsOpen == false)
            {
                if (!roomDictionay.ContainsKey(room.Name)) continue;

                Destroy(roomDictionay[room.Name].gameObject);
                roomDictionay.Remove(room.Name);
            }
            // 새로 생성된 방
            else if (!roomDictionay.ContainsKey(room.Name))
            {
                LSY_RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomDictionay.Add(room.Name, roomEntry);
                roomEntry.SetRoomInfo(room);
            }
            // 방 정보가 변경된 경우
            else if (roomDictionay.ContainsKey(room.Name))
            {
                LSY_RoomEntry roomEntry = roomDictionay[room.Name];
                roomEntry.SetRoomInfo(room);
            }
        }
    }


    public void ClearRoomEntries()
    {
        foreach (string name in roomDictionay.Keys)
        {
            Destroy(roomDictionay[name].gameObject);
        }
        roomDictionay.Clear();
    }
}

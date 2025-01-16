using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_LobbyPanel : LSY_BaseUI
{
    [Header("CreateRoomPanel")]
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject checkImage;
    [SerializeField] TMP_Text errorText;

    [Header("RoomPanel")]
    [SerializeField] RectTransform roomContent;
    [SerializeField] LSY_RoomEntry roomEntryPrefab;

    [Header("닉네임 변경")]
    [SerializeField] TMP_InputField nickName;
    [SerializeField] TMP_Text warningText;
    [SerializeField] TMP_Text confirmText;

    [Header("검색 패널")]
    [SerializeField] private TMP_InputField searchInputField;

    private Dictionary<string, LSY_RoomEntry> roomDictionay = new Dictionary<string, LSY_RoomEntry>();
    private bool isPasswordProtected = false;

    public const string lsy_RoomName = "LSY_TestRoom";

    private void Start()
    {
        BindAll();

        isPasswordProtected = false;

        PhotonNetwork.LocalPlayer.NickName = $"Player{Random.Range(1000, 10000)}";
        nickName.text = PhotonNetwork.LocalPlayer.NickName;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.EnableCloseConnection = true; // 플레이어 퇴장 기능 활성화

        searchInputField.onValueChanged.AddListener(OnSearchRoom); 
    }

    #region 게임 종료
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    #endregion

    #region 방 검색하기 기능
    public void OnSearchRoom(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            foreach (var room in roomDictionay)
            {
                LSY_RoomEntry roomEntry = room.Value;
                roomEntry.gameObject.SetActive(true);
            }
        }
        SearchRoomText(searchText.ToLower());
    }

    public void SearchRoomText(string searchText)
    {
        // KeyValuePair: 딕셔너리의 각 항목의 키와 값을 동시에 다룰 수 있는 구조체
        foreach (KeyValuePair<string, LSY_RoomEntry> roomEntry in roomDictionay)
        {
            string roomName = roomEntry.Key.ToLower(); // 방 이름
            LSY_RoomEntry entry = roomEntry.Value; // 방을 나타내는 UI

            entry.gameObject.SetActive(roomName.Contains(searchText)); // 방 이름에 검색한 글자가 포함될 경우 해당 UI를 활성화 시켜줌
        }
    }

    #endregion

    #region 닉네임 변경 기능

    public void NicknameChange()
    {
        string nickname = nickName.text;

        if (nickname.Length < 2 || nickname.Length > 10)
        {
            StartCoroutine(WarningTextRoutine());
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = nickname;
        StartCoroutine(ConfirmTextRoutine());
        nickName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    IEnumerator WarningTextRoutine()
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        warningText.gameObject.SetActive(false);
        nickName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    IEnumerator ConfirmTextRoutine()
    {
        confirmText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        confirmText.gameObject.SetActive(false);
    }

    #endregion

    #region 방 생성하기
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
            StartCoroutine(ErrorTextRoutine("방 이름을 입력해주세요"));
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

        if (password.Length < 2 && isPasswordProtected)
        {
            StartCoroutine(ErrorTextRoutine("비밀번호는 두자리 이상이여야 합니다"));
            return;
        }

        PhotonNetwork.CreateRoom(roomName, options);
        Debug.Log("방 생성 완료");
        GetUI("CreateRoomPanel").gameObject.SetActive(false);
        RoomPanelInit();
    }

    IEnumerator ErrorTextRoutine(string text)
    {
        errorText.text = text;
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        errorText.gameObject.SetActive(false);
    }

    public void RoomPanelInit()
    {
        isPasswordProtected = false;
        passwordInputField.text = "";
        passwordInputField.gameObject.SetActive(false);
        checkImage.gameObject.SetActive(false);
    }
    #endregion

    #region 방 업데이트
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

        SearchRoomText(searchInputField.text.ToLower());
    }


    public void ClearRoomEntries()
    {
        foreach (string name in roomDictionay.Keys)
        {
            Destroy(roomDictionay[name].gameObject);
        }
        roomDictionay.Clear();
    }
    #endregion
}

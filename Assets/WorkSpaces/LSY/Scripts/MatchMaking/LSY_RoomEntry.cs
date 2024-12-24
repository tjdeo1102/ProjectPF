using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_RoomEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _currentPlayer;
    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private Image lockImage;

    private bool isLock;
    private string roomPassword;

    [SerializeField] private GameObject passwordPopUpPrefab;
    private GameObject passwordPopUpInstance;

    private void Start()
    {
        _joinRoomButton.onClick.AddListener(JoinRoom);
    }

    public void SetRoomInfo(RoomInfo info)
    {
        if (info == null)
        {
            return;
        }

        _roomName.text = info.Name;
        _currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        _joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;

        if (info.CustomProperties != null && info.CustomProperties.ContainsKey("IsPasswordProtected"))
        {
            bool isPasswordProtected = (bool)info.CustomProperties["IsPasswordProtected"];

            if (isPasswordProtected)
            {
                isLock = true;

                roomPassword = (string)info.CustomProperties["RoomPassword"];
                Debug.Log("Room Password: " + roomPassword);
                lockImage.gameObject.SetActive(true);
            }
            else
            {
                lockImage.gameObject.SetActive(false);
                isLock = false;
            }
        }
        else
        {
            Debug.Log("커스텀 프로퍼티가 존재하지 않음");
        }
    }

    public void JoinRoom()
    {
        if (isLock)
        {
            ShowPasswordPopUp();
        }
        else
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(_roomName.text);
        }
    }

    private void ShowPasswordPopUp()
    {
        if (passwordPopUpInstance == null)
        {
            passwordPopUpInstance = Instantiate(passwordPopUpPrefab);

            Canvas worldSpaceCanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();

            if (worldSpaceCanvas != null && worldSpaceCanvas.renderMode == RenderMode.WorldSpace)
            {
                passwordPopUpInstance.transform.SetParent(worldSpaceCanvas.transform, false);
            }

            var passwordPopupScript = passwordPopUpInstance.GetComponent<LSY_PasswordPopUp>();
            if (passwordPopupScript != null)
            {
                passwordPopupScript.OnPasswordSubmitEvent += OnPasswordSubmit;
                passwordPopupScript.OnPasswordCancelEvent += OnPasswordCancel;
            }
        }

        passwordPopUpInstance.SetActive(true);
    }


    public void OnPasswordSubmit(string enteredPassword)
    {
        if (enteredPassword == roomPassword)
        {
            passwordPopUpInstance.SetActive(false);
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(_roomName.text);
        }
        else
        {
            Debug.LogWarning("Incorrect password!");
            var passwordPopupScript = passwordPopUpInstance.GetComponent<LSY_PasswordPopUp>();
            if (passwordPopupScript != null)
            {
                passwordPopupScript.ClearInputField();
            }
        }
    }

    public void OnPasswordCancel()
    {
        passwordPopUpInstance.SetActive(false);
    }
}

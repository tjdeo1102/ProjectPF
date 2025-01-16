using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity.Demos;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LSY_PlayerEntry : LSY_BaseUI
{
    [Header("룸 플레이어 세팅")]
    [SerializeField] private TMP_Text readyText;       
    [SerializeField] private TMP_Text nameText;        
    [SerializeField] private Button readyButton;

    [Header("다른 플레이어 버튼")]
    [SerializeField] private Button playerButton;

    [Header("방장 이미지")]
    [SerializeField] private GameObject hostImage;

    [Header("추방&방장위임 팝업")]
    [SerializeField] private LSY_RoomPopUp roomPopUp;

    [SerializeField] Image characterImgae;

    Color normalColor;    
    Color pressedColor;   
    
    public bool _isReady;

    public Player player;  

    public void Init(Player player)
    {
        this.player = player;
    }

    public void PlayerClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (player != null)
            {
                roomPopUp.TogglePopup(player);
                Debug.Log("Player: " + player.NickName);
            }
        }
    }


    private void Start()
    {
        characterImgae.gameObject.SetActive(false);
        hostImage.SetActive(false);
        pressedColor = new Color(0.372549f, 0.7137255f, 0.2509804f, 1);
        normalColor = new Color(0.8490566f, 0.8490566f, 0.8490566f, 1);
        readyButton.onClick.AddListener(ReadyButton);
    }

    // 레디 버튼 클릭 시 호출되는 함수
    public void ReadyButton()
    {
        _isReady = !_isReady;   
        Ready();               
    }

    public void SetPlayer(Player player)
    {
        roomPopUp.HidePopup();
        Init(player);

        if (player.IsMasterClient)
        {
            nameText.text = player.NickName;
            hostImage.SetActive(true);
            playerButton.interactable = true;
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                playerButton.interactable = false;
            }
        }
        else
        {
            playerButton.interactable = false;
            nameText.text = player.NickName;
            hostImage.SetActive(false);
        }

        if (PhotonNetwork.LocalPlayer == player)
        {
            nameText.color = Color.yellow;
        }
        else
        {
            nameText.color = Color.white;
        }

        characterImgae.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;

        if (player.GetReady())
        {
            readyText.text = "준비 완료";
            readyButton.GetComponent<Image>().color = pressedColor;
        }
        else
        {
            readyText.text = "준비";
            readyButton.GetComponent<Image>().color = normalColor;
        }
    }

    public void SetEmpty()
    {
        nameText.text = "";
        readyText.text = "";
        readyButton.gameObject.SetActive(false);
        characterImgae.gameObject.SetActive(false);
        hostImage.SetActive(false);
    }

    public void Ready()
    {
        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }

        UpdateButtonState();  
    }

    private void UpdateButtonState()
    {
        if (_isReady)
        {
            readyText.text = "준비 완료";
            readyButton.GetComponent<Image>().color = pressedColor;
        }
        else
        {
            readyText.text = "준비";
            readyButton.GetComponent<Image>().color = normalColor;
        }
    }
}

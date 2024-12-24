using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_PlayerEntry : LSY_BaseUI
{
    [SerializeField] private TMP_Text readyText;       
    [SerializeField] private Button readyButton;       
    [SerializeField] private TMP_Text nameText;        
    [SerializeField] private GameObject hostImage;      

    Color normalColor;    
    Color pressedColor;      
    private bool _isCheck;                              
    public bool _isReady;

    private void Start()
    {
        hostImage.SetActive(false);
        pressedColor = new Color(0.372549f, 0.7137255f, 0.2509804f, 1);
        normalColor = new Color(0.8490566f, 0.8490566f, 0.8490566f, 1);
    }

    private void Update()
    {
        readyButton.onClick.AddListener(ReadyButton);
    }

    // 레디 버튼 클릭 시 호출되는 함수
    public void ReadyButton()
    {
        _isReady = !_isReady;  
        UpdateButtonState();    
        Ready();               
    }

    public void SetPlayer(Player player)
    {

        if (player.IsMasterClient)
        {
            nameText.text = player.NickName;
            hostImage.SetActive(true);
        }
        else
        {
            nameText.text = player.NickName;
            hostImage.SetActive(false);
        }

        if (PhotonNetwork.LocalPlayer.NickName == nameText.text)
        {
            _isCheck = true;
        }

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

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_RoomPopUp : MonoBehaviour
{
    [Header("팝업 패널")]
    [SerializeField] private GameObject popupPanel;
    [Header("방장위임 버튼")]
    [SerializeField] private Button transferButton;
    [Header("추방 버튼")]
    [SerializeField] private Button kickButton;
    [Header("추방 되었을 때 팝업")]

    private Player selectedPlayer;              

    void Start()
    {
        kickButton.onClick.AddListener(KickPlayer);
        transferButton.onClick.AddListener(TransferHost);
    }

    public void TogglePopup(Player player)
    {
        selectedPlayer = player;

        bool isActive = !popupPanel.activeSelf;
        popupPanel.SetActive(isActive);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    public void KickPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CloseConnection(selectedPlayer);
            HidePopup();
        }
    }


    public void TransferHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(selectedPlayer);
            HidePopup();
        }
    }
}

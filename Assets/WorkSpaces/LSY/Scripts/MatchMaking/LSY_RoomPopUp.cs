using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_RoomPopUp : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel; 
    [SerializeField] private Button kickButton;    
    [SerializeField] private Button transferButton;
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
        PhotonNetwork.EnableCloseConnection = true;


        if (PhotonNetwork.IsMasterClient)
        {
            if (selectedPlayer == null)
            {
                Debug.LogError("선택된 플레이어 없음");
                return;
            }

            PhotonNetwork.CloseConnection(selectedPlayer);

            HidePopup();
            Debug.Log("마스터가 추방");
        }
    }


    public void TransferHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(selectedPlayer);
            HidePopup();
            Debug.Log("마스터가 방장넘김");
        }
        Debug.Log("방장넘김");
    }
}

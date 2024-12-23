using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LSY_PlayerEntry : LSY_BaseUI
{
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private TMP_Text _nameText;
    private bool _isCheck;
    public bool _isReady;

    private void Update()
    {
        if (_isCheck == true && Input.GetKeyDown(KeyCode.F5))
        {
            Ready();
        }
    }

    public void SetPlayer(Player player)
    {
        if (player.IsMasterClient)
        {
            _nameText.text = player.NickName;
            _nameText.color = Color.yellow;
        }
        else
        {
            _nameText.text = player.NickName;
            _nameText.color = Color.black;
        }

        if (PhotonNetwork.LocalPlayer.NickName == _nameText.text)
        {
            _isCheck = true;
        }
        /*_readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;*/

        if (player.GetReady())
        {
            _readyText.text = "Ready";
        }
        else
        {
            _readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        _nameText.text = "";
        _readyText.text = "";
    }

    public void Ready()
    {
        // 레디가 아니었으면 레디시키기
        // 레디가 맞았으면 레디 풀어주기
        bool ready = PhotonNetwork.LocalPlayer.GetReady();

        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(false);

        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }
}

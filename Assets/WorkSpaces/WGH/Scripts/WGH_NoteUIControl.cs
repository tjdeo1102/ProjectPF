using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WGH_NoteUIControl : MonoBehaviourPun
{
    [SerializeField] Image[] images;

    // E_WGH_NoteType 열거형을 넣으면 해당 노트 UI 조절
    public void OnUI(E_WGH_NoteType noteType)
    {
        photonView.RPC("SetUI", RpcTarget.All, noteType, true);
    }
    
    public void OffUI(E_WGH_NoteType noteType)
    {
        photonView.RPC("SetUI", RpcTarget.All, noteType, false);
    }

    [PunRPC]
    private void SetUI(E_WGH_NoteType noteType, bool isOn)
    {
        images[(int)noteType - 1].gameObject.SetActive(isOn);
    }
}

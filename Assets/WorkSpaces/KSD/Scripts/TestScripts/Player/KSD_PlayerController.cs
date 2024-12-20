using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_PlayerController : MonoBehaviourPunCallbacks
{
    [Header("네트워크 플레이어 기본설정")]
    [SerializeField] List<Behaviour> disableScripts;
    private void Awake()
    {
        if (photonView.Owner.IsLocal == false)
        {
            for (int i = 0; i < disableScripts.Count; i++)
            {
                disableScripts[i].enabled = false;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} has left the room.");

        if (photonView.Owner == otherPlayer)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

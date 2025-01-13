using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_PunCallbacks : MonoBehaviourPunCallbacks
{
    // 다른 플레이어 나간 경우, 같이 나가짐 (저장하지 않고 종료)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        KSD_GameManager.Instance.Quit(true,true,false);
    }
}

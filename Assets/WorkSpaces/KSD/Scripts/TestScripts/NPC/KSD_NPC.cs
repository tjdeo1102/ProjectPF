using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_NPC : MonoBehaviourPun
{
    [Header("NPC 기본 설정")]
    [SerializeField] private Transform patrolPosition;
    [SerializeField] private float patrolDuration;

    private bool isFinishLoop;

    private void Start()
    {
        isFinishLoop = true;
    }

    [PunRPC]
    public void FinishOrderRPC(PhotonMessageInfo info)
    {
        if (isFinishLoop)
        {
            // 쏜 사람만 호출해도 되는 부분
            if (info.Sender.IsLocal)
            {
                KSD_GameManager.Instance.AddFinishPlayerCount(1);
            }

            // 모든 사람에게 적용되야 하는 부분
            isFinishLoop = false;
            Debug.Log("<color=white> 손님 주문 처리 완료 </color>");
            transform.DOMove(patrolPosition.position, patrolDuration)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => { Debug.Log("<color=blue> 새로운 손님 도착 </color>"); isFinishLoop = true; });
        }
    }

    public void FinishOrder()
    {
        photonView.RPC("FinishOrderRPC", RpcTarget.All);
    }
}

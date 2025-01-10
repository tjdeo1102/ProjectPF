using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_NetworkGrabInteractable : XRGrabInteractable
{
    private bool isGrabInNetwork;
    private PhotonView view;
    private InteractionLayerMask originLayer;

    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
        view = GetComponent<PhotonView>();
        originLayer = interactionLayers;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);

            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            view.RPC("ChangeRigidbodySetting", RpcTarget.AllViaServer, true);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (view.Owner == PhotonNetwork.LocalPlayer 
            && isGrabInNetwork == true)
        {
            base.OnSelectExited(args);

            view.TransferOwnership(PhotonNetwork.MasterClient);
            view.RPC("ChangeRigidbodySetting", RpcTarget.AllViaServer, false);
        }
    }

    [PunRPC]
    public void ChangeRigidbodySetting(bool isSelect, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;

        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (isSelect) 
        {
            interactionLayers = 2;
        }
        else
        {
            interactionLayers = originLayer;
        }
        
    }
}

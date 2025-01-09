using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class WGH_XRGrabInteractable : XRGrabInteractable
{
    private bool isGrabInNetwork;

    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);

            interactable.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.All, true);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (interactable.Owner == PhotonNetwork.LocalPlayer
            && isGrabInNetwork == true)
        {
            base.OnSelectExited(args);

            interactable.TransferOwnership(PhotonNetwork.MasterClient);
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void ChangeRigidbodySetting(bool isSelect, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;
        interactionLayers = InteractionLayerMask.GetMask("SmellStick");
        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        rigid.useGravity = isSelect;
        interactionLayers = InteractionLayerMask.GetMask("Default");
        interactionLayers &= ~InteractionLayerMask.GetMask("SmellStick");
    }
}

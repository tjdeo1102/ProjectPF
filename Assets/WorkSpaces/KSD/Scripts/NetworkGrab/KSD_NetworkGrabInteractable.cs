using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_NetworkGrabInteractable : XRGrabInteractable
{
    private PhotonView view;
    public int OriginLayer;
    public Rigidbody OriginTransform;
    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
        view = GetComponent<PhotonView>();
        OriginTransform = GetComponent<Rigidbody>();
        OriginLayer = interactionLayers.value;
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
        base.OnSelectEntered(args);
        view.TransferOwnership(PhotonNetwork.LocalPlayer);
        //print("소유권 양도");
        view.RPC("OnChangeRigidbodySetting", RpcTarget.AllViaServer, OriginLayer);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
        base.OnSelectExited(args);
        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (view.Owner == PhotonNetwork.LocalPlayer)
        {
            // view.TransferOwnership(PhotonNetwork.MasterClient);
            view.RPC("OffChangeRigidbodySetting", RpcTarget.AllViaServer, OriginLayer);
        }
    }
    [PunRPC]
    public void OnChangeRigidbodySetting(int originLayer, PhotonMessageInfo info)
    {
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (!view.IsMine)
        {
            interactionLayers = InteractionLayerMask.GetMask("DontInteract");
            OriginTransform.isKinematic = true;
        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            OriginTransform.isKinematic = false;
            Debug.Log($"{originLayer} 잡았다!");
        }
    }
    [PunRPC]
    public void OffChangeRigidbodySetting(int originLayer, PhotonMessageInfo info)
    {
        if (!view.IsMine)
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            OriginTransform.isKinematic = false;
        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
        }
    }
}
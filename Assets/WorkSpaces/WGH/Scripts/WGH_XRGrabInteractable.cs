using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class WGH_XRGrabInteractable : XRGrabInteractable
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
        if (view.IsMine)
        { // 내가 소유한 물체인 경우
            OriginLayer = interactionLayers.value;
        }
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor == false)
        {
            base.OnSelectEntered(args);
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            view.RPC("OnChangeRigidbodySetting", RpcTarget.All, OriginLayer);
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor == false)
        {
            base.OnSelectExited(args);
            // 본인이 잡고있던 물체인 경우에만 놓도록 설정
            if (view.Owner == PhotonNetwork.LocalPlayer)
            {
                view.RPC("OffChangeRigidbodySetting", RpcTarget.All, OriginLayer);
            }
        }
    }
    [PunRPC]
    public void OnChangeRigidbodySetting(int originLayer, PhotonMessageInfo info)
    {
        OriginTransform.useGravity = false;
        StartCoroutine(gravityRoutine());
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (!view.IsMine)
        {
            interactionLayers = InteractionLayerMask.GetMask("DontInteract");

        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
        }
    }
    [PunRPC]
    public void OffChangeRigidbodySetting(int originLayer, PhotonMessageInfo info)
    {
        if (!view.IsMine)
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };

        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            OriginTransform.useGravity = true;
        }
    }

    IEnumerator gravityRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        OriginTransform.useGravity = false;
        yield break;
    }
}

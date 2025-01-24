using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSH_XRGrabInteractable2 : XRGrabInteractable
{
    private PhotonView view;
    public int OriginLayer;
    public Rigidbody OriginTransform;

    private bool isKinematic; // 체크 용

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
        base.OnSelectEntered(args);

        if (args.interactorObject is XRSocketInteractor == false)
        {
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            view.RPC("RPC_OnSelectEntered", RpcTarget.AllViaServer, OriginLayer);
        }
    }

    [PunRPC]
    public void RPC_OnSelectEntered(int originLayer)
    {
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (!view.Controller.IsLocal)
        {
            interactionLayers = InteractionLayerMask.GetMask("DontInteract");
            OriginTransform.isKinematic = true;
            Debug.Log($" 잡았을때 다른사람");
        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            OriginTransform.isKinematic = false;
            Debug.Log($" 잡았을때 자기자신");
        }
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);

        if (args.interactorObject is XRSocketInteractor == false)
        {
            // 본인이 잡고있던 물체인 경우에만 놓도록 설정
            if (view.Owner == PhotonNetwork.LocalPlayer)
            {
                // view.TransferOwnership(PhotonNetwork.MasterClient);
                view.RPC("RPC_OnSelectExiting", RpcTarget.AllViaServer, OriginLayer);
            }
        }
    }

    [PunRPC]
    public void RPC_OnSelectExiting(int originLayer)
    {
        if (!view.Controller.IsLocal)
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            OriginTransform.isKinematic = false;
            Debug.Log($" 놓아을때 다른사람");
        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
            Debug.Log($" 놓아을때 자기자신");
        }
    }
}
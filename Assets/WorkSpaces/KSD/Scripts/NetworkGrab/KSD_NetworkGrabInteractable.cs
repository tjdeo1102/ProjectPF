using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_NetworkGrabInteractable : XRGrabInteractable, IPunObservable
{
    private PhotonView view;
    public int OriginLayer;
    public Rigidbody OriginTransform;
    private bool originGravity;
    private bool originKinematic;
    private bool isSelectCheck;
    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
        view = GetComponent<PhotonView>();
        OriginTransform = GetComponent<Rigidbody>();
        OriginLayer = interactionLayers.value;
        originGravity = OriginTransform.useGravity;
        originKinematic = OriginTransform.isKinematic;
    }

    public void Update()
    {
        // 잡고있는 인터렉터가 소켓인 경우에는 제3자의 것이므로 물리 활성화
        if (firstInteractorSelecting is XRSocketInteractor)
        {
            OriginTransform.useGravity = originGravity;
            OriginTransform.isKinematic = originKinematic;
            return;
        }
        // 잡고 있지 않는 상태면 원래 물리 속성으로 복귀
        if (!isSelectCheck)
        {
            OriginTransform.useGravity = originGravity;
            OriginTransform.isKinematic = originKinematic;
        }
        else
        {
            OriginTransform.useGravity = false;
            OriginTransform.isKinematic = true;
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isSelected);
        }
        else
        {
            // stream의 Count가 비어있는 경우에는 ReceiveNext 에러
            // 해당 에러 예외처리 (1인 이유는 보내는 값이 한개이므로)
            if (stream.Count < 1) return;
            isSelectCheck = (bool)stream.ReceiveNext();
        }
    }
}
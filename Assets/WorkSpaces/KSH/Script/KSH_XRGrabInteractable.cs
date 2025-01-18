using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSH_XRGrabInteractable : XRGrabInteractable
{
    [SerializeField] PhotonView photonView;
    private int originLayer;

    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        { // 내가 소유한 물체인 경우
            originLayer = interactionLayers.value;
        }
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        // base.OnSelectEntering(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectEntering(args);
        }
        else
        {
            PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
            photonView.RPC(nameof(RPC_SelectEntering), RpcTarget.AllViaServer, interactorPV.ViewID);
        }
    }

    [PunRPC]
    public void RPC_SelectEntering(int interactorID)
    {
        SelectEnterEventArgs args = new SelectEnterEventArgs();
        args.interactorObject = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>();
        args.interactableObject = this;
        args.manager = interactionManager;

        base.OnSelectEntering(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // base.OnSelectEntered(args);
        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectEntered(args);
        }
        else if (!isSelected)
        {
            PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
            photonView.RPC(nameof(RPC_SelectEntered), RpcTarget.AllViaServer, interactorPV.ViewID);
        }
    }

    [PunRPC]
    public void RPC_SelectEntered(int interactorID)
    {
        SelectEnterEventArgs args = new SelectEnterEventArgs();
        args.interactorObject = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>();
        args.interactableObject = this;
        args.manager = interactionManager;
        if (!photonView.Controller.IsLocal)
        {
            // 상호작용 레이어 변경
            interactionLayers = InteractionLayerMask.GetMask("DontInteract");
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // base.OnSelectExiting(args);
        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectExiting(args);
        }
        else if (isSelected)
        {
            PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
            photonView.RPC(nameof(RPC_SelectExiting), RpcTarget.AllViaServer, interactorPV.ViewID, args.isCanceled, originLayer);
        }
    }

    [PunRPC]
    public void RPC_SelectExiting(int interactorID, bool isCanceled, int originLayer)
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>();
        args.interactableObject = this;
        args.manager = interactionManager;
        args.isCanceled = isCanceled;

        if (!photonView.Controller.IsLocal)
        {
            // 레이어 변경
            interactionLayers = new InteractionLayerMask { value = originLayer };
        }

        base.OnSelectExiting(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // base.OnSelectExited(args);

        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectExited(args);
        }
        else
        {
            PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
            photonView.RPC(nameof(RPC_SelectExited), RpcTarget.AllViaServer, interactorPV.ViewID, args.isCanceled);
        }
    }

    [PunRPC]
    public void RPC_SelectExited(int interactorID, bool isCanceled)
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>();
        args.interactableObject = this;
        args.manager = interactionManager;
        args.isCanceled = isCanceled;

        base.OnSelectExited(args);
    }
}


//    private PhotonView view;
//    public int OriginLayer;

//    private bool isKinematic; // 체크 용

//    protected override void Awake()
//    {
//        base.Awake();
//        movementType = MovementType.VelocityTracking;
//        view = GetComponent<PhotonView>();

//        if (view.IsMine)
//        { // 내가 소유한 물체인 경우
//            OriginLayer = interactionLayers.value;
//        }
//    }

//    protected override void OnSelectEntering(SelectEnterEventArgs args)
//    {
//        if (args.interactorObject is XRSocketInteractor)
//        {
//            base.OnSelectEntering(args);
//        }
//        else
//        {

//        }
//    }

//    protected override void OnSelectEntered(SelectEnterEventArgs args)
//    {
//        base.OnSelectEntered(args);

//        if (args.interactorObject is XRSocketInteractor == false)
//        {
//            view.TransferOwnership(PhotonNetwork.LocalPlayer);
//            //print("소유권 양도");
//            view.RPC("OnChangeRigidbodySetting", RpcTarget.AllViaServer, OriginLayer);
//        }
//    }

//    protected override void OnSelectExited(SelectExitEventArgs args)
//    {
//        base.OnSelectExited(args);
//        if (args.interactorObject is XRSocketInteractor == false)
//        {
//            // 본인이 잡고있던 물체인 경우에만 놓도록 설정
//            if (view.Owner == PhotonNetwork.LocalPlayer)
//            {
//                // view.TransferOwnership(PhotonNetwork.MasterClient);
//                view.RPC("OffChangeRigidbodySetting", RpcTarget.AllViaServer, OriginLayer);
//            }
//        }
//    }

//    [PunRPC]
//    public void RPC_SelectEntering(int originLayer, PhotonMessageInfo info)
//    {
//        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
//        // 다른 유저가 상호작용 못하도록 레이어 변경
//        if (!view.Controller.IsLocal)
//        {
//            interactionLayers = InteractionLayerMask.GetMask("DontInteract");
//            //OriginTransform.isKinematic = true;
//            Debug.Log($" 잡았을때 다른사람");
//        }
//        else
//        {
//            //interactionLayers = new InteractionLayerMask { value = originLayer };
//            //OriginTransform.isKinematic = false;
//            Debug.Log($" 잡았을때 자기자신");
//        }
//    }

//    [PunRPC]
//    public void OffChangeRigidbodySetting(int originLayer, PhotonMessageInfo info)
//    {
//        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
//        if (!view.Controller.IsLocal)
//        {
//            interactionLayers = new InteractionLayerMask { value = originLayer };
//            //OriginTransform.isKinematic = false;
//            Debug.Log($" 놓아을때 다른사람");
//        }
//        else
//        {
//            //interactionLayers = new InteractionLayerMask { value = originLayer };
//            Debug.Log($" 놓아을때 자기자신");
//        }
//    }
//}
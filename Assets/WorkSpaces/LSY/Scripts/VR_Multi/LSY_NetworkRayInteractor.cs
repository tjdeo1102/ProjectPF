using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class LSY_NetworkRayInteractor : XRRayInteractor
{
    [SerializeField] PhotonView photonView;

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
       // base.OnSelectEntering(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SelectEntering), RpcTarget.AllViaServer, interactablePV.ViewID);
    }

    [PunRPC]
    public void RPC_SelectEntering(int interactableID)
    {
        SelectEnterEventArgs args = new SelectEnterEventArgs();
        args.interactorObject = this;
        args.interactableObject = PhotonView.Find(interactableID).GetComponent<IXRSelectInteractable>();
        args.manager = interactionManager;

        base.OnSelectEntering(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Entering -> {args.interactableObject.transform.gameObject.name}");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // base.OnSelectEntered(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SelectEntered), RpcTarget.AllViaServer, interactablePV.ViewID);
        interactablePV.RequestOwnership();
    }

    [PunRPC]
    public void RPC_SelectEntered(int interactableID)
    {
        SelectEnterEventArgs args = new SelectEnterEventArgs();
        args.interactorObject = this;
        args.interactableObject = PhotonView.Find(interactableID).GetComponent<IXRSelectInteractable>();
        args.manager = interactionManager;

        base.OnSelectEntered(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Entered -> {args.interactableObject.transform.gameObject.name}");
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // base.OnSelectExiting(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SelectExiting), RpcTarget.AllViaServer, interactablePV.ViewID, args.isCanceled);
    }

    [PunRPC]
    public void RPC_SelectExiting(int interactableID, bool isCanceled)
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = this;
        args.interactableObject = PhotonView.Find(interactableID).GetComponent<IXRSelectInteractable>();
        args.manager = interactionManager;
        args.isCanceled = isCanceled;

        base.OnSelectExiting(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Exiting -> {args.interactableObject.transform.gameObject.name}");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // base.OnSelectExited(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        photonView.RPC(nameof(RPC_SelectExited), RpcTarget.AllViaServer, interactablePV.ViewID, args.isCanceled);
    }

    [PunRPC]
    public void RPC_SelectExited(int interactableID, bool isCanceled)
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = this;
        args.interactableObject = PhotonView.Find(interactableID).GetComponent<IXRSelectInteractable>();
        args.manager = interactionManager;
        args.isCanceled = isCanceled;

        base.OnSelectExited(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Exited -> {args.interactableObject.transform.gameObject.name}");
    }
}

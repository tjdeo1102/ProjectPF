using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class LSY_NetworkGrabInteractable : XRGrabInteractable
{
    [SerializeField] PhotonView photonView;
    private float originMass;
    private float originDrag;
    private float originAngularDrag;
    private Rigidbody rb;
    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        originMass = rb.mass;
        originDrag = rb.drag;
        originAngularDrag = rb.angularDrag;
    }

    private void Update()
    {
        // 놓은 상태일 때, 지속적으로 물리버그 픽스
        if (isSelected == false)
        {
            rb.mass = originMass;
            rb.drag = originDrag;
            rb.angularDrag = originAngularDrag;
            rb.useGravity = true;
            rb.isKinematic = false;
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
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Entering -> {args.interactorObject.transform.gameObject.name}");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // base.OnSelectEntered(args);
        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectEntered(args);
        }
        else
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

        base.OnSelectEntered(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Entered -> {args.interactorObject.transform.gameObject.name}");
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // base.OnSelectExiting(args);
        if (args.interactorObject is XRSocketInteractor)
        {
            base.OnSelectExiting(args);
        }
        else
        {
            PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
            photonView.RPC(nameof(RPC_SelectExiting), RpcTarget.AllViaServer, interactorPV.ViewID, args.isCanceled);
        }
    }

    [PunRPC]
    public void RPC_SelectExiting(int interactorID, bool isCanceled)
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        args.interactorObject = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>();
        args.interactableObject = this;
        args.manager = interactionManager;
        args.isCanceled = isCanceled;

        base.OnSelectExiting(args);
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Exiting -> {args.interactorObject.transform.gameObject.name}");
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
        Debug.Log($"{photonView.Owner} {gameObject.name} Select Exited -> {args.interactorObject.transform.gameObject.name}");
    }
}

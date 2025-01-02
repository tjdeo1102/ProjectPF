using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DispendorActive : XRBaseInteractable
{ 
    public LSY_DispensorLiquid dispensorLiquid;
    [SerializeField] Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        dispensorLiquid.OnSelectedEnter();
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}

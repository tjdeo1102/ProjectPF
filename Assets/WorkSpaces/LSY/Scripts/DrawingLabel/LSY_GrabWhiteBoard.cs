using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabWhiteBoard : XRGrabInteractable
{
    Rigidbody rb;
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;
    Vector3 originPosition;
    Quaternion originRotation;

    bool setLabel = false;

    private void Start()
    {
        enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (setLabel == false)
        {
            boxCollider.enabled = true;
            meshCollider.enabled = false;
            setLabel = true;
        }
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

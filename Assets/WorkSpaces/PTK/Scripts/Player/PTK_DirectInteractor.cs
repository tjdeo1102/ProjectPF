using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_DirectInteractor : XRDirectInteractor
{
    [SerializeField] PhotonView photonView;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        //interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        //interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}

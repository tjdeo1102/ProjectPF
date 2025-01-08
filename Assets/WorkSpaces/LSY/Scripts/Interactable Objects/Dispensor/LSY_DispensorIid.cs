using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DispensorIid : XRBaseInteractable
{
    public LSY_DispensorLiquid dispensorLiquid;

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (dispensorLiquid.isLitOpen == false)
        {
            dispensorLiquid.isLitOpen = true;
            dispensorLiquid.photonView.RPC("RPC_PlayAnimation", RpcTarget.All, "LidOn");
        }
        else
        {
            dispensorLiquid.isLitOpen = false;
            dispensorLiquid.photonView.RPC("RPC_PlayAnimation", RpcTarget.All, "LidOff");
            dispensorLiquid.photonView.RPC("RPC_PlayAnimation", RpcTarget.All, "LidIdle");
        }

        interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}

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
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.OpenDispenser);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidOn", true);
        }
        else
        {
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.CloseDispenser);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidOff", false);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidIdle", false);
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

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DispensorIid : XRBaseInteractable
{
    public LSY_DispensorLiquid dispensorLiquid;
    PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (dispensorLiquid.isLitOpen == false)
        {
            photonView.RPC("OpenSound", RpcTarget.All);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidOn", true);
        }
        else
        {
            photonView.RPC("CloseSound", RpcTarget.All);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidOff", false);
            dispensorLiquid.photonView.RPC("RPC_LitAnimation", RpcTarget.All, "LidIdle", false);
        }

        interactablePV.RequestOwnership();
    }

    [PunRPC]
    public void OpenSound()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.OpenDispenser);
    }

    [PunRPC]
    public void CloseSound()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.CloseDispenser);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}

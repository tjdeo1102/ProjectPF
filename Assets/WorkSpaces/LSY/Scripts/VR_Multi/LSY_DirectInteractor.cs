using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DirectInteractor : XRDirectInteractor
{
    [SerializeField] private Animator animator;
    PhotonView photonView;

    protected override void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (animator != null)
        {
            animator.SetTrigger("Grab");
            photonView.RPC("RPC_PlaySfx", RpcTarget.All, 5);
        }

        // 잡은 사실을 네트워크를 통해서 전달
        // 잡은 플레이어가 잡은 사실을 네트워크를 통해 전달
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (interactablePV != null)
            interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (animator != null)
        {
            animator.SetTrigger("Release");
        }

        // 놓은 플레이어가 잡은 물체의 소유권을 방장에게 다시 주기
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (interactablePV != null)
            interactablePV.TransferOwnership(PhotonNetwork.MasterClient);

    }

    [PunRPC]
    private void RPC_PlaySfx(int sfx)
    {
        KSH_AudioManager.Instance.PlaySfx((KSH_AudioManager.Sfx)sfx);
    }
}

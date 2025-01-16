using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_RayInteractor : XRRayInteractor
{
    [SerializeField] private Animator animator;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (animator != null)
        {
            animator.SetTrigger("Grab");
            //KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Pick_up);
        }
        if (args.interactableObject.transform.TryGetComponent<KSD_NetworkGrabInteractable>(out var com)) return;

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
        if (args.interactableObject.transform.TryGetComponent<KSD_NetworkGrabInteractable>(out var com)) return;

        // 놓은 플레이어가 잡은 물체의 소유권을 방장에게 다시 주기
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (interactablePV != null)
            interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}
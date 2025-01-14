using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabWhiteBoard : XRGrabInteractable
{
    Rigidbody rb;
    [SerializeField] GameObject labelPrefab;
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;
    Vector3 originPosition;
    Quaternion originRotation;
    private bool isGrabInNetwork;
    [SerializeField] public Transform spawnTransform;

    bool setLabel = false;

    private void Start()
    {
        enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (args.interactorObject is XRSocketInteractor)
            return;
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (setLabel == false)
        {
            boxCollider.enabled = true;
            meshCollider.enabled = false;
            setLabel = true;
            PhotonNetwork.Instantiate("Label", spawnTransform.position, spawnTransform.rotation);
        }
        if (isGrabInNetwork == false)
        {
             interactablePV.RPC("ChangeRigidbodySetting", RpcTarget.All);
        }
        interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
        base.OnSelectExited(args);
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        if (isGrabInNetwork == true)
        {
            interactablePV.RPC("ChangeRigidbodySetting2", RpcTarget.All);
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Label_on);
        }
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }

    [PunRPC]
    public void ChangeRigidbodySetting(PhotonMessageInfo info)
    {
        isGrabInNetwork = true; 
        interactionLayers = InteractionLayerMask.GetMask("Default") | InteractionLayerMask.GetMask("Label");
        if (info.Sender.IsLocal) return;
        interactionLayers = InteractionLayerMask.GetMask("Default");
        interactionLayers &= ~InteractionLayerMask.GetMask("Label");
    }

    [PunRPC]
    public void ChangeRigidbodySetting2(PhotonMessageInfo info)
    {
        isGrabInNetwork = false;
        interactionLayers = InteractionLayerMask.GetMask("Default") | InteractionLayerMask.GetMask("Label");
    }

}

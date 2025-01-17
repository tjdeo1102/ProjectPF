using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_XRSocketInteractor : XRSocketInteractor
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        KSH_XRGrabInteractable interactable = args.interactableObject as KSH_XRGrabInteractable;
        if (interactable != null)
        {
            interactable.interactionLayers = new InteractionLayerMask { value = interactable.OriginLayer };
            Debug.Log($"{interactable.OriginLayer} 소켓 값");
            interactable.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.Log("KSH_XRGrabInteractable이 아닙니다.");
        }
    }
}

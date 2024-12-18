using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_CustomInteractor : XRBaseInteractable
{
    [SerializeField] Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        transform.parent = args.interactorObject.transform;
        rb.isKinematic = true;
        rb.useGravity = false;
       // transform.localPosition = Vector3.zero;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        transform.parent = null;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

}

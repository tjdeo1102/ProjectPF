using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabLabel : XRGrabInteractable
{
    public LSY_Label lsy_label;
    bool doneLabel = false;
    public GameObject label;
    public GameObject whiteBoard;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (doneLabel == false)
        {
            label.SetActive(false);
            whiteBoard.SetActive(true);
            lsy_label.OnWhiteBoard();
            doneLabel = true;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    }
}

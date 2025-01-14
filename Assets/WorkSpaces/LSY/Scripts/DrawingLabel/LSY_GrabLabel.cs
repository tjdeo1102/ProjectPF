using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabLabel : XRGrabInteractable
{
    public LSY_Label lsy_label;
    bool doneLabel = false;
    public GameObject label;
    public GameObject whiteBoard;
    public Rigidbody rb;

    public PhotonView photonView;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if (!doneLabel)
        {
            photonView.RPC("Grab", RpcTarget.All);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    }

    [PunRPC]
    public void Grab()
    {
        label.SetActive(false);    
        whiteBoard.SetActive(true);
        lsy_label.OnWhiteBoard();  
        doneLabel = true;        
    }
}

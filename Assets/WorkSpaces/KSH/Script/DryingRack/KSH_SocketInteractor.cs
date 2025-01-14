using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_SocketInteractor : XRSocketInteractor
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (PhotonNetwork.IsMasterClient)
        {

        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (PhotonNetwork.IsMasterClient)
        {

        }
    }

    [PunRPC]
    private void RPC_OnSelect()
    {
        
    }
}
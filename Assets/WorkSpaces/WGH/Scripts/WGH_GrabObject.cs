using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WGH_GrabObject : MonoBehaviour
{
    private PhotonView photonView;
    Rigidbody rigid;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(gameObject.GetComponent<Rigidbody>() != null )
        {
            rigid = gameObject.GetComponent<Rigidbody>();
        }
    }
    /// <summary>
    /// 잡았을 때
    /// </summary>
    public void OnGrab()
    {
        if (photonView != null && PhotonNetwork.IsConnected)
        {
            // 소유권을 현재 로컬 플레이어로 전환
            photonView.RequestOwnership();
            photonView.RPC("GravityRPC", RpcTarget.All, true);
        }
    }

    [PunRPC]
    public void GravityRPC(bool isGrabbed)
    {
        rigid.useGravity = !isGrabbed;
    }

    public void OnRelease()
    {
        
    }
}

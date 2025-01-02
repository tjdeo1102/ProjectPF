using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WGH_GrabObject : MonoBehaviourPun
{
    private WGH_SmellStick smellStick;
    Rigidbody rigid;

    private void Awake()
    {
        smellStick = GetComponent<WGH_SmellStick>();
    }
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 물체를 잡았을 때 호출
    /// </summary>
    public void OnGrab()
    {
        photonView.RPC("GravityRPC", RpcTarget.All, true);
    }

    /// <summary>
    /// 물체를 놓았을 때 호출
    /// </summary>
    public void OnRelease()
    {
        photonView.RPC("GravityRPC", RpcTarget.All, false);
    }


    /// <summary>
    /// 중력 상태 동기화
    /// </summary>
    [PunRPC]
    public void GravityRPC(bool isGrabbed)
    {
        if (rigid != null)
        {
            rigid.useGravity = !isGrabbed;
            smellStick.isGrab = isGrabbed;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WGH_GrabObject : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    Rigidbody rigid;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        rigid = GetComponent<Rigidbody>();

        Debug.Log($"초기 소유권 상태: IsMine={photonView.IsMine}");
    }

    /// <summary>
    /// 물체를 잡았을 때 호출
    /// </summary>
    public void OnGrab()
    {
        // 소유권 변경 요청
        if (!photonView.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // MasterClient가 직접 소유권 변경
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            else
            {
                // MasterClient에게 소유권 변경 요청
                photonView.RPC("RequestOwnershipFromMaster", RpcTarget.MasterClient);
            }
        }

        // 모든 클라이언트에서 동작 적용
        photonView.RPC("GravityRPC", RpcTarget.All, true);
    }

    /// <summary>
    /// 물체를 놓았을 때 호출
    /// </summary>
    public void OnRelease()
    {
        // 모든 클라이언트에서 동작 적용
        photonView.RPC("GravityRPC", RpcTarget.All, false);
    }

    /// <summary>
    /// MasterClient에서 소유권 변경 처리
    /// </summary>
    [PunRPC]
    public void RequestOwnershipFromMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
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
        }
    }
}

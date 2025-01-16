using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSH_XRGrabInteractable : XRGrabInteractable
{
    private bool isGrabInNetwork;
    private PhotonView view;
    private int originLayer;

    protected override void Awake()
    {
        base.Awake();
        movementType = MovementType.VelocityTracking;
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        { // 내가 소유한 물체인 경우
            originLayer = interactionLayers.value;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);

            view.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            view.RPC("OnChangeRigidbodySetting", RpcTarget.AllViaServer, true, originLayer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (view.Owner == PhotonNetwork.LocalPlayer
            && isGrabInNetwork == true)
        {
            base.OnSelectExited(args);

            // view.TransferOwnership(PhotonNetwork.MasterClient);
            view.RPC("OffChangeRigidbodySetting", RpcTarget.AllViaServer, false, originLayer);
        }
    }

    [PunRPC]
    public void OnChangeRigidbodySetting(bool isSelect, int originLayer, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;
        Debug.Log("OnChangeRigidbodySetting");
        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        Debug.Log($"{info.Sender.NickName}, A 호출");
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (isSelect)
        {
            interactionLayers = InteractionLayerMask.GetMask("DontInteract");
        }
        else
        {
            interactionLayers = new InteractionLayerMask { value = originLayer };
        }
    }

    [PunRPC]
    public void OffChangeRigidbodySetting(bool isSelect, int originLayer, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;
        Debug.Log("OffChangeRigidbodySetting");
        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        Debug.Log($"{info.Sender.NickName}, B 호출");
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        // 다른 유저가 상호작용 못하도록 레이어 변경
        interactionLayers = new InteractionLayerMask { value = originLayer };
    }
}
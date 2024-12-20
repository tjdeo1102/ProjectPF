using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSD_NetworkGrabInteractable : XRGrabInteractable
{
    //[Header("네트워크 설정")]
    private bool isGrabInNetwork;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);

            interactable.RequestOwnership();
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.All, true);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (interactable.Owner == PhotonNetwork.LocalPlayer 
            && isGrabInNetwork == true)
        {
            base.OnSelectExited(args);

            interactable.TransferOwnership(PhotonNetwork.MasterClient);
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.AllViaServer, false);
        }
    }

    [PunRPC]
    public void ChangeRigidbodySetting(bool isSelect)
    {
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        isGrabInNetwork = isSelect;

        //var view = GetComponent<PhotonView>();
        //// 본인이 아니라면 상용작용 못하는 물체로 만들기
        //if (isSelect
        //    && view.IsMine == false)
        //{

        //}
        //else
        //{
        //    // 물체를 놓을 때는 상호작용 상태 활성화
        //    intera
        //}

    }

    //[PunRPC]
    //public void SelectControlRPC(bool isSelect, PhotonMessageInfo info)
    //{
    //    // 1. 잡으라는 RPC가 날라온 경우, 해당 RPC가 잡을 수 있는 상태인지 확인
    //    if (isGrabInNetwork)
    //    {
    //        // 2. 잡고 있는 경우라면 해당 잡고 있는 대상이 본인인지 확인
    //        if (info.Sender.IsLocal
    //            && isSelect == false)
    //        {
    //            // 3. 잡고 있는 사람이 본인이고 놓으려고 시도한다면 놓도록 만들어줌

    //            isGrabInNetwork = false;
    //        }
    //        else
    //        {
    //            // 4. 잡고 있는 사람이 본인이 아닌데 잡으려고 시도하면 반응 없음 
    //            Debug.Log("<color=#ecd59c> 이미 누군가 잡고 있습니다. </color>");
    //        }
    //    }
    //    else
    //    {
    //        // 5. 아무도 안잡고 있는 경우에 잡으려고 한다면 잡도록 만들어줌
    //        if (isSelect == true)
    //        {

    //            isGrabInNetwork = true;
    //        }
    //        else
    //        {
    //            // 아무도 잡지 않았는데, 해당 물건을 이미 잡고있다가 놓은 상황이라면 오류
    //            Debug.LogError("<color=red> 잡은 적이 없는 물체를 놓을 수는 없습니다. </color>\"");
    //        }
    //    }
    //}


}

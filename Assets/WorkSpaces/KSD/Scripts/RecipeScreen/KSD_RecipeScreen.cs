using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_RecipeScreen : XRBaseInteractable
{
    [Header("기본 설정")]
    public float OriginSize;
    public float TargetSize;
    [SerializeField] GameObject screen;

    [Header("움직이는 범위 제한 (월드 좌표 기준)")]
    public Transform MinTransform;
    public Transform MaxTrasnform;
    [Range(0f, 1f)] public float Value;

    [Header("원래 위치 복귀 속도 설정")]
    public float ReturnPositonVelocity;

    private bool isGrabInNetwork;
    private Transform selectInteractor;
    private PhotonView photonView;
    private float directionY;
    private Vector3 MinPos;
    private Vector3 MaxPos;
    private Vector3 lastPosition;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        MinPos = MinTransform.position;
        MaxPos = MaxTrasnform.position;
        directionY = (MaxPos - MinPos).normalized.y;
        lastPosition = transform.position;
    }

    private void Update()
    {
        var newY = lastPosition.y;
        if (isGrabInNetwork && selectInteractor != null)
        {
            // 1. 컨트롤러의 위치 추적
            newY = selectInteractor.position.y;
            // 2. 범위를 넘어간 Y값 조정
            if (MinPos.y > MaxPos.y) newY = Mathf.Clamp(newY,MaxPos.y,MinPos.y);
            else newY = Mathf.Clamp(newY,MinPos.y,MaxPos.y);

            var newPos = new Vector3(lastPosition.x, newY, lastPosition.z);
            lastPosition = newPos;
        }
        else
        {
            newY += -directionY * Time.deltaTime * ReturnPositonVelocity;
            // 2. 범위를 넘어간 Y값 조정
            if (MinPos.y > MaxPos.y) newY = Mathf.Clamp(newY, MaxPos.y, MinPos.y);
            else newY = Mathf.Clamp(newY, MinPos.y, MaxPos.y);

            var newPos = new Vector3(lastPosition.x, newY, lastPosition.z);
            lastPosition = newPos;
        }

        Value = Mathf.Abs((lastPosition.y - MinPos.y) / (MaxPos.y - MinPos.y));
        var lerp = OriginSize + (TargetSize - OriginSize) * Value;
        transform.localScale = new Vector3(transform.localScale.x, lerp, transform.localScale.z);

        // 펼치고 있는 사람은 보이지 않고, 펴는 사람만 보이도록 구현
        if (screen != null )
        {
            if (photonView.IsMine == false
                && isGrabInNetwork == true)
            {
                if (Value > 0.9f) screen.SetActive(true);
                else screen.SetActive(false);
            }
            else screen.SetActive(false);
        }
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);
            selectInteractor = args.interactorObject.transform;

            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            photonView.RPC("ChangeRigidbodySetting", RpcTarget.AllViaServer, true);
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
            selectInteractor = null;
            interactable.TransferOwnership(PhotonNetwork.MasterClient);
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.AllViaServer, false);
        }
    }

    [PunRPC]
    public void ChangeRigidbodySetting(bool isSelect, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;

        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
    }
}

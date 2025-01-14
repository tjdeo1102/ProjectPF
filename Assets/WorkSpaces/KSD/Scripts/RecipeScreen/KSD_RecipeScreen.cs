using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_RecipeScreen : XRBaseInteractable, IPunObservable
{
    [Header("기본 설정")]
    public float OriginSize;
    public float TargetSize;
    public Vector3 OriginHandlePosition;
    public Vector3 TargetHandlePosition;
    public float DeltaSize;
    public float MoveDirectionY;
    [SerializeField] private Image screen;
    [SerializeField] private Transform screenTransform;
    [SerializeField] private Transform handleTransform;

    [Range(0f, 1f)] public float Value;


    [Header("원래 위치 복귀 속도 설정")]
    public float ReturnVelocity;

    private bool isGrabInNetwork;
    private Transform selectInteractor;
    private PhotonView photonView;
    private float lastPositionY;
    private InteractionLayerMask originLayer;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        originLayer = interactionLayers;
        screenTransform.localScale = new Vector3(screenTransform.localScale.x, OriginSize, screenTransform.localScale.z);
        handleTransform.localPosition = OriginHandlePosition;
    }

    private void Update()
    {
        // 자신이 소유한 경우에만 물리 계산, 그렇지 않은 경우에는 동기화된 값을 통해 업데이트 
        var dif = TargetSize - OriginSize;
        if (photonView.IsMine)
        {
            var newSize = screenTransform.localScale.y;
            if (isGrabInNetwork && selectInteractor != null)
            {
                // 1. 컨트롤러의 위치 추적
                var moveY = selectInteractor.position.y - lastPositionY;
                lastPositionY = selectInteractor.position.y;
                // 2. 추가할 사이즈 계산
                // 2-1. 같은 방향으로 증가한 경우, 사이즈값 추가
                var y = MoveDirectionY;
                if (y < 0) y = -y;

                if (moveY * MoveDirectionY > 0)
                {
                    newSize += DeltaSize * Time.deltaTime * y;
                }
                // 2-2. 다른 방향으로 증가한 경우, 사이즈값 감소
                else if (moveY * MoveDirectionY < 0)
                {
                    newSize -= DeltaSize * Time.deltaTime * y;
                }
                if (OriginSize < TargetSize) newSize = Mathf.Clamp(newSize, OriginSize, TargetSize);
                else newSize = Mathf.Clamp(newSize, TargetSize, OriginSize);
            }
            else
            {
                // 1. 감소할 사이즈량 계산
                var y = MoveDirectionY;
                if (y < 0) y = -y;
                y *= dif / Mathf.Abs(dif);
                newSize -= (y * Time.deltaTime * ReturnVelocity);
                if (OriginSize < TargetSize) newSize = Mathf.Clamp(newSize, OriginSize, TargetSize);
                else newSize = Mathf.Clamp(newSize, TargetSize, OriginSize);
            }

            Value = Mathf.Abs((newSize - OriginSize) / dif);
        }

        screenTransform.localScale = new Vector3(screenTransform.localScale.x, Value * dif + OriginSize, screenTransform.localScale.z);
        handleTransform.localPosition = OriginHandlePosition + (TargetHandlePosition - OriginHandlePosition) * Value;
        //// 펼치고 있는 사람은 보이지 않고, 펴는 사람만 보이도록 구현
        //if (photonView.IsMine == false)
        //{
        //    screen.fillAmount = Value;
        //}
        //else screen.fillAmount = 0;
        screen.fillAmount = Value;
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);
            selectInteractor = args.interactorObject.transform;
            lastPositionY = selectInteractor.position.y;
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
        // 다른 유저가 상호작용 못하도록 레이어 변경
        if (isSelect)
        {
            interactionLayers = 2;
        }
        else
        {
            interactionLayers = originLayer;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 소유권 있는 자신은 Value를 보내기만 하기
        if (stream.IsWriting && photonView.IsMine == true)
        {
            stream.SendNext(Value);
        }
        // 소유권 없는 상대방은 Value를 받기만 하기
        else if (stream.IsReading && photonView.IsMine == false)
        {
            Value = (float)stream.ReceiveNext();
        }
    }
}

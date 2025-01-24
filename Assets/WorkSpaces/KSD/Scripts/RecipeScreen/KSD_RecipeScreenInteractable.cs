using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView))]
public class KSD_RecipeScreenInteractable : XRBaseInteractable, IPunObservable
{
    [Header("기본 설정")]
    public float OriginSize;
    public float TargetSize;
    public Vector3 OriginHandlePosition;
    public Vector3 TargetHandlePosition;
    public float DeltaSize;
    [SerializeField] private Image[] screens;
    [SerializeField] private Transform screenTransform;
    [SerializeField] private Transform handleTransform;

    [Range(0f, 1f)] public float Value;


    [Header("원래 위치 복귀 속도 설정")]
    public float ReturnVelocity;


    private PhotonView photonView;
    private float lastPositionY;
    private int originLayer;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        originLayer = interactionLayers.value;
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
            if (firstInteractorSelecting != null)
            {
                var interactor = firstInteractorSelecting.transform;
                // 1. 컨트롤러의 위치 추적
                var moveY = interactor.position.y - lastPositionY;
                lastPositionY = interactor.position.y;
                // 2. 추가할 사이즈 계산
                // 2-1. y값 감소하는 무빙인 경우, 사이즈값 추가
                if (moveY < 0)
                {
                    newSize += DeltaSize * Time.deltaTime;
                }
                else if (moveY > 0)
                {
                    newSize -= DeltaSize * Time.deltaTime;
                }
                if (OriginSize < TargetSize) newSize = Mathf.Clamp(newSize, OriginSize, TargetSize);
                else newSize = Mathf.Clamp(newSize, TargetSize, OriginSize);
            }
            else
            {
                // 1. 감소할 사이즈량 계산
                newSize -= (Time.deltaTime * ReturnVelocity);
                if (OriginSize < TargetSize) newSize = Mathf.Clamp(newSize, OriginSize, TargetSize);
                else newSize = Mathf.Clamp(newSize, TargetSize, OriginSize);
            }

            Value = Mathf.Abs((newSize - OriginSize) / dif);
        }

        screenTransform.localScale = new Vector3(screenTransform.localScale.x, Value * dif + OriginSize, screenTransform.localScale.z);
        handleTransform.localPosition = OriginHandlePosition + (TargetHandlePosition - OriginHandlePosition) * Value;
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].fillAmount = Value;
        }
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 소유자가 없는 경우에만 물건을 잡도록 설정
        base.OnSelectEntered(args);
        lastPositionY = args.interactorObject.transform.position.y;
        //photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        ////print("소유권 양도");
        //photonView.RPC("OnChangeRigidbodySetting", RpcTarget.AllViaServer, originLayer);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        //// 본인이 잡고있던 물체인 경우에만 놓도록 설정
        //if (photonView.Owner == PhotonNetwork.LocalPlayer)
        //{
        //    base.OnSelectExited(args);
        //    selectInteractor = null;
        //    photonView.RPC("OffChangeRigidbodySetting", RpcTarget.AllViaServer, originLayer);
        //}
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

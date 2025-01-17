using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
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
    [SerializeField] private Image[] screens;
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
            if (selectInteractor != null)
            {
                // 1. 컨트롤러의 위치 추적
                var moveY = selectInteractor.position.y - lastPositionY;
                lastPositionY = selectInteractor.position.y;
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

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        PhotonView interactorPV = args.interactorObject.transform.GetComponent<PhotonView>();
        photonView.RPC("SetDisableLayer", RpcTarget.AllViaServer, interactorPV.ViewID);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        selectInteractor = null;
        photonView.RPC("SetEnableLayer", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void SetDisableLayer(int interactorID)
    {
        SelectEnterEventArgs args = new SelectEnterEventArgs();
        selectInteractor = PhotonView.Find(interactorID).GetComponent<IXRSelectInteractor>().transform;
        base.OnSelectEntered(args);

        if (photonView.IsMine) return;
        print("다른 사람 것이므로 비활성화");
        interactionLayers = InteractionLayerMask.GetMask("DontInteract");
    }

    [PunRPC]
    public void SetEnableLayer()
    {
        SelectExitEventArgs args = new SelectExitEventArgs();
        selectInteractor = null;
        base.OnSelectExited(args);
        print("활성화");
        interactionLayers = new InteractionLayerMask() { value = originLayer };
    }
}

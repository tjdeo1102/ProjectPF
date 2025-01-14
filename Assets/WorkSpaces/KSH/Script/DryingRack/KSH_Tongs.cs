using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class KSH_Tongs : XRGrabInteractable
{
    [SerializeField] public List<XRGrabInteractable> OverlappingObjects = new List<XRGrabInteractable>();
    [SerializeField] private Transform gripPoint; // 집게 오브젝트가 잡을 중심점

    private PhotonView photonView;
    private Collider iscollider;
    private bool isActivated = false;


    private bool isGrabInNetwork;

    protected override void Awake()
    {
        base.Awake();
    }
    

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        iscollider = GetComponent<Collider>();
        iscollider.enabled = false;
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        isActivated = true; // 누르고 있는 상태 활성화
        iscollider.enabled = true;
        Debug.Log("집게 활성화!");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 소유자가 없는 경우에만 물건을 잡도록 설정
        if (isGrabInNetwork == false)
        {
            base.OnSelectEntered(args);

            interactable.TransferOwnership(PhotonNetwork.LocalPlayer);
            //print("소유권 양도");
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.All, true);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            return;
        var interactable = args.interactableObject.transform.GetComponent<PhotonView>();

        // 본인이 잡고있던 물체인 경우에만 놓도록 설정
        if (interactable.Owner == PhotonNetwork.LocalPlayer
            && isGrabInNetwork == true)
        {
            base.OnSelectExited(args);

            interactable.TransferOwnership(PhotonNetwork.MasterClient);
            interactable.RPC("ChangeRigidbodySetting", RpcTarget.All, false);
        }
    }

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        isActivated = false; // 누르고 있는 상태 비활성화
        Debug.Log("집게 비활성화!");

        // 손을 뗐을 때 동작
        photonView.RPC("ReleaseObjects", RpcTarget.All);
    }

    private void Update()
    {
        if (isActivated)
        {
            if (OverlappingObjects.Count > 0)
            {
                // 상태가 변경되었을 때만 RPC 호출
                photonView.RPC("GrabObjects", RpcTarget.All);
            }
            GrabObjects(); // 로컬에서 동작
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        XRGrabInteractable xrGrab = other.GetComponent<XRGrabInteractable>();
        if (other.gameObject.CompareTag("Ingredient"))
        {
            if (!OverlappingObjects.Contains(xrGrab))
            {
                OverlappingObjects.Add(xrGrab);
                Debug.Log($"추가된 오브젝트: {xrGrab.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        XRGrabInteractable xrGrab = other.GetComponent<XRGrabInteractable>();
        if (OverlappingObjects.Contains(xrGrab))
        {
            OverlappingObjects.Remove(xrGrab);
            Debug.Log($"제거된 오브젝트: {xrGrab.name}");
        }
    }

    [PunRPC]
    public void ChangeRigidbodySetting(bool isSelect, PhotonMessageInfo info)
    {
        isGrabInNetwork = isSelect;
        interactionLayers = InteractionLayerMask.GetMask("Fowers");
        // 잡은 경우에, 잡은 사람 빼고, 물리 비활성화
        if (info.Sender.IsLocal) return;
        var rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = isSelect;
        rigid.useGravity = isSelect;
        interactionLayers = InteractionLayerMask.GetMask("Default");
        interactionLayers &= ~InteractionLayerMask.GetMask("Fowers");
    }

    [PunRPC]
    private void GrabObjects()
    {
        foreach (var obj in OverlappingObjects)
        {
            Debug.Log($"잡은 오브젝트: {obj.name}");
            obj.transform.position = gripPoint.position;
            obj.transform.rotation = gripPoint.rotation;
            obj.transform.parent = gripPoint;
            obj.GetComponent<XRGrabInteractable>().enabled = false;
            obj.GetComponent<Rigidbody>().isKinematic = true; // 물리 비활성화
        }
    }

    [PunRPC]
    private void ReleaseObjects()
    {
        // 소켓 할당 처리
        KSH_SocketSorting socketSorting = FindObjectOfType<KSH_SocketSorting>();
        if (socketSorting != null && socketSorting.IsSockets)
        {
            socketSorting.HandleReleasedObjects(OverlappingObjects);
        }

        foreach (var obj in OverlappingObjects)
        {
            Debug.Log($"해제된 오브젝트: {obj.name}");
            obj.transform.parent = null;
            obj.GetComponent<XRGrabInteractable>().enabled = true;
            obj.GetComponent<Rigidbody>().isKinematic = false; // 물리 활성화
        }

        OverlappingObjects.Clear();
        iscollider.enabled = false;
    }
}
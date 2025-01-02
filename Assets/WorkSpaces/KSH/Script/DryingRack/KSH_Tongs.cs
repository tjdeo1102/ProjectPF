using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_Tongs : XRGrabInteractable
{
    [SerializeField] public List<XRGrabInteractable> OverlappingObjects = new List<XRGrabInteractable>();
    [SerializeField] private Transform gripPoint; // 집게 오브젝트가 잡을 중심점

    private Collider iscollider;
    private bool isActivated = false;

    private void Start()
    {
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

    protected override void OnDeactivated(DeactivateEventArgs args)
    {
        base.OnDeactivated(args);
        isActivated = false; // 누르고 있는 상태 비활성화
        Debug.Log("집게 비활성화!");

        // 손을 뗐을 때 동작
        ReleaseObjects();
    }

    private void Update()
    {
        if (isActivated)
        {
            GrabObjects(); // 누르고 있는 동안 동작
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
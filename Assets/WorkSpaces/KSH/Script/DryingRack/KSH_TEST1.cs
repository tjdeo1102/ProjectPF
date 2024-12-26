using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_TEST1 : MonoBehaviour
{
    // 모든 자식들의 소켓 목록
    [SerializeField] private XRSocketInteractor[] sockets;

    private void Awake()
    {
        // 모든 자식 소켓들을 한 번만 가져와 저장합니다.
        sockets = GetComponentsInChildren<XRSocketInteractor>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거된 객체의 부모를 가져옵니다.
        Transform parent = other.transform.parent;
        if (parent == null) return; // 부모가 없으면 종료

        // 부모 객체 내에서 XRGrabInteractable 컴포넌트를 가진 자식 오브젝트들을 가져옵니다.
        XRGrabInteractable[] interactables = parent.GetComponentsInChildren<XRGrabInteractable>();

        // 자식 오브젝트들에게 부딫친 오브젝트들을 하나 씩 붙여주기
    }
}
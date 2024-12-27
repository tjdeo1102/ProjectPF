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
        Debug.Log(parent);
        if (parent == null) return; // 부모가 없으면 종료

        // 부모 객체 내에서 XRGrabInteractable 컴포넌트를 가진 자식 오브젝트들을 가져옵니다.
        XRGrabInteractable[] interactables = parent.GetComponentsInChildren<XRGrabInteractable>(true);

        // 빈 소켓들을 리스트로 관리
        Queue<XRSocketInteractor> emptySockets = GetEmptySockets();

        // 빈 소켓이 없으면 처리 중단
        if (emptySockets.Count == 0) return;

        // 각 XRGrabInteractable을 빈 소켓에 할당
        foreach (XRGrabInteractable interactable in interactables)
        {
            if (emptySockets.Count == 0) break; // 소켓이 더 이상 없으면 중단

            XRSocketInteractor socket = emptySockets.Dequeue();
            AssignInteractableToSocket(socket, interactable); // 소켓에 오브젝트를 할당
        }
    }

    // 비어 있는 소켓을 Queue로 반환하는 메서드
    private Queue<XRSocketInteractor> GetEmptySockets()
    {
        Queue<XRSocketInteractor> emptySockets = new Queue<XRSocketInteractor>();

        foreach (XRSocketInteractor socket in sockets)
        {
            // 대체 API를 사용하여 빈 소켓인지 확인
            if (!socket.hasSelection)
            {
                emptySockets.Enqueue(socket);
            }
        }

        return emptySockets;
    }

    // 소켓에 객체를 할당하는 메서드
    private void AssignInteractableToSocket(XRSocketInteractor socket, XRGrabInteractable interactable)
    {
        // 소켓의 위치로 오브젝트를 이동
        interactable.transform.position = socket.transform.position;
        interactable.transform.rotation = socket.transform.rotation;

        // Rigidbody를 초기화하여 이동 중 충돌 방지
        Rigidbody rb = interactable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"객체 {interactable.name}이 소켓 {socket.name}에 배치되었습니다.");
    }
}
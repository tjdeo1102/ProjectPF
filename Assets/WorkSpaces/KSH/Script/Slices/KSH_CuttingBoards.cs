using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_CuttingBoards : MonoBehaviour
{
    // 소켓에 물체가 있는지 확인용
    private XRSocketInteractor xrSocket;
    // 건조된 물체인지 확인용
    private KSH_SlicesCheck slicesCheck;


    private void Awake()
    {
        xrSocket = GetComponent<XRSocketInteractor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 빈 소켓인지 확인
        if (xrSocket.hasSelection)
        {
            slicesCheck = other.GetComponent<KSH_SlicesCheck>();
            if (slicesCheck == null)
            {
                Debug.Log($"KSH_SlicesCheck가 없는 오브젝트: {other.gameObject.name}");
                return;
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && slicesCheck.IsSlices == false)
            {
                rb.useGravity = true;
            }
        }
    }
}

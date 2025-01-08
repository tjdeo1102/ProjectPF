using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class KSD_BaseTool : MonoBehaviour
{
    public Transform ResetPosition;
    public float MaxDistance;


    private XRGrabInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
    }
    // Update is called once per frame
    void Update()
    {
        // 물건을 놓았을 때만, 제자리 돌아가는 기능 활성화
        if (interactable.isSelected) return;

        if (Vector3.Distance(ResetPosition.position,transform.position) > MaxDistance)
        {
            transform.position = ResetPosition.position;
        }
    }
}

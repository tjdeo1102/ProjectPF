using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 상호작용 콜라이더 탐지용 클래스
public class WGH_InteractArea : MonoBehaviour
{
    [SerializeField] WGH_NPCController controller;
    
    public event Action OnChangedSmellStick;

    private void Awake()
    {
        controller = GetComponentInParent<WGH_NPCController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_SmellStick smellStick))
        {
            controller.SmellStick = smellStick;
            OnChangedSmellStick?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_SmellStick smellStick))
        {
            controller.SmellStick = null;
        }
    }
}

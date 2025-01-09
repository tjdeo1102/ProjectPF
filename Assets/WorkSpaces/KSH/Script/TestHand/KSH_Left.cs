using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KSH_Left : MonoBehaviour
{
    [SerializeField] InputActionReference trigger;

    [SerializeField] Animator animator;

    private void OnEnable()
    {
        trigger.action.Enable();
    }

    private void Update()
    {
        float triggerValue = trigger.action.ReadValue<float>();
        animator.SetLayerWeight(1, triggerValue);
    }

    private void OnDisable()
    {
        trigger.action.Disable();
    }
}

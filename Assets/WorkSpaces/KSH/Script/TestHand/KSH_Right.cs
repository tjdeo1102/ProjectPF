using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KSH_Right : MonoBehaviour
{
    [SerializeField] InputActionReference grip;

    [SerializeField] Animator animator;

    private void OnEnable()
    {
        grip.action.Enable();
    }

    private void Update()
    {
        float triggerValue = grip.action.ReadValue<float>();
        animator.SetLayerWeight(1, triggerValue);
    }

    private void OnDisable()
    {
        grip.action.Disable();
    }
}

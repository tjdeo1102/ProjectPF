using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_Handle : MonoBehaviour
{
    [SerializeField] private XRKnob knob; // XRKnob 컴포넌트 참조

    void Update()
    {
        if (knob != null)
        {
            float currentValue = knob.value; // 노브의 현재 값
            Debug.Log($"Current Knob Value: {currentValue}");
        }
    }
}

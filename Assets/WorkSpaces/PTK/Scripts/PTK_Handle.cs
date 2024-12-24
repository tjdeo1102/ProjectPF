using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_Handle : MonoBehaviour
{
    [SerializeField] private XRKnob knob;
    [SerializeField] private float turnResult = 10f;
    private float lastValue;

    public UnityEvent MixDone;

    void Start()
    {
        lastValue = knob.value;
    }

    void Update()
    {
        if (knob != null)
        {
            float currentValue = knob.value;
            float delta = Mathf.Abs(currentValue - lastValue);

            if (delta >= turnResult)
            {
                MixDone?.Invoke();
                lastValue = currentValue;
            }
        }
    }
}

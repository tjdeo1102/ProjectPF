using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_Handle : MonoBehaviourPun
{
    [SerializeField] private XRKnob knob;
    [SerializeField] private float turnResult = 10f;
    private float lastValue;

    public UnityEvent mixDone;

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
                photonView.RPC("RPC_MixDone", RpcTarget.All);
                lastValue = currentValue;
            }
        }
    }

    [PunRPC]
    private void RPC_MixDone()
    {
        mixDone.Invoke();
    }
}

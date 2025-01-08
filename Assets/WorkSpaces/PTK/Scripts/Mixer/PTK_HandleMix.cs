using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_HandleMix : MonoBehaviourPun
{
    [SerializeField] private XRKnob Knob;
    [SerializeField] private float turnResult = 5f;

    public PTK_HandleElec elec;

    private float lastValue;
    private bool wasSecondHandleActive = false;
    public UnityEvent mixDone;

    void Start()
    {
        lastValue = Knob.value;
    }

    void Update()
    {
        if (wasSecondHandleActive == false && elec.isSecondHandleActive)
        {
            lastValue = Knob.value;
        }

        wasSecondHandleActive = elec.isSecondHandleActive;

        if (elec.isSecondHandleActive == true)
        {
            float currentValue = Knob.value;
            float delta = Mathf.Abs(currentValue - lastValue);

            if (delta >= turnResult)
            {
                Debug.Log("RPC_MixDone");
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
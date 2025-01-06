using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_HandleElec : MonoBehaviourPun
{
    [SerializeField] private XRKnob Knob;
    [SerializeField] private float decayRate = 1f;
    [SerializeField] private float secondaryKnobThreshold = 5f;
    
    public bool isSecondHandleActive = false;

    void Update()
    {
        CheckSecondKnob();
    }

    private void CheckSecondKnob()
    {
        if (Knob.value > 0)
        {
            Knob.value -= decayRate * Time.deltaTime;
            Knob.value = Mathf.Max(Knob.value, 0);
        }

        bool newState;

        if (Knob.value > secondaryKnobThreshold)
        {
            newState = true;
        }
        else
        {
            newState = false;
        }

        if (newState != isSecondHandleActive)
        {
            isSecondHandleActive = newState;
            photonView.RPC("RPC_UpdateSecondHandleState", RpcTarget.All, newState);
        }
    }

    [PunRPC]
    private void RPC_UpdateSecondHandleState(bool newState)
    {
        isSecondHandleActive = newState;
    }
}
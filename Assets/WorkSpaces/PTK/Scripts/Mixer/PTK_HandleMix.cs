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
    private bool isSoundPlaying = false;
    public UnityEvent mixDone;

    private float previousKnobValue;

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

            if (!Mathf.Approximately(Knob.value, previousKnobValue))
            {
                if (!isSoundPlaying)
                {
                    photonView.RPC("RPC_PlaySfx", RpcTarget.All, 11);
                    isSoundPlaying = true;
                }
            }
            else
            {
                if (isSoundPlaying)
                {
                    photonView.RPC("RPC_StopInputSfx", RpcTarget.All, 11);
                    isSoundPlaying = false;
                }
            }

            if (delta >= turnResult)
            {
                Debug.Log("RPC_MixDone");
                photonView.RPC("RPC_MixDone", RpcTarget.All);
                lastValue = currentValue;
            }

            previousKnobValue = Knob.value;
        }
        else
        {
            if (isSoundPlaying)
            {
                photonView.RPC("RPC_StopInputSfx", RpcTarget.All, 11);
                isSoundPlaying = false;
            }
        }
    }
   
    [PunRPC]
    private void RPC_MixDone()
    {
        mixDone.Invoke();
    }

    [PunRPC]
    private void RPC_PlaySfx(int sfx)
    {
        KSH_AudioManager.Instance.PlaySfx((KSH_AudioManager.Sfx)sfx);
    }

    [PunRPC]
    private void RPC_StopInputSfx(int sfx)
    {
        KSH_AudioManager.Instance.StopInputSfx((KSH_AudioManager.Sfx)sfx);
    }
}
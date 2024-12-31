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
    //[SerializeField] private float turnResult;
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
            // knob범위가 0~1이므로, 0 또는 1인 경우에만 작동하도록 설정
            float currentValue = knob.value;
            //float delta = Mathf.Abs(currentValue - startValue);

            //if (delta >= turnResult)
            if (currentValue == 1 || currentValue == 0)
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

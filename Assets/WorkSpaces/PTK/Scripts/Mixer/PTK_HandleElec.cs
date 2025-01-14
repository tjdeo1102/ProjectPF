using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PTK_HandleElec : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private XRKnob Knob;
    [SerializeField] private float decayRate = 1f;
    [SerializeField] private float secondaryKnobThreshold = 5f;

    public float knobMaxValue = 10f;

    public bool isSecondHandleActive = false;

    public bool isCheatModeActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CheatModeOn();
            Debug.Log("CheatOn!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CheatModeOff();
            Debug.Log("CheatOff!");
        }

        CheckSecondKnob();
    }

    private void CheckSecondKnob()
    {
        if (isCheatModeActive)
            return;

        if (photonView.IsMine == false)
            return;

        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Blender_handle);

        Knob.value = Mathf.Clamp(Knob.value, 0, knobMaxValue);

        if (Knob.value > 0)
        {
            Knob.value -= decayRate * Time.deltaTime;
            Knob.value = Mathf.Max(Knob.value, 0);
        }

        bool newState = Knob.value > secondaryKnobThreshold;

        if (newState != isSecondHandleActive)
        {
            isSecondHandleActive = newState;
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Blender_play);
        }
    }

    public float GetKnobValue()
    {
        return Knob.value;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Knob.value);
            stream.SendNext(isSecondHandleActive);
        }
        else if (stream.IsReading)
        {
            Knob.value = (float)stream.ReceiveNext();
            isSecondHandleActive = (bool)stream.ReceiveNext();
        }
    }

    public void CheatModeOn()
    {
        isCheatModeActive = true;
        isSecondHandleActive = true;
    }

    public void CheatModeOff()
    {
        isCheatModeActive = false;
    }
}
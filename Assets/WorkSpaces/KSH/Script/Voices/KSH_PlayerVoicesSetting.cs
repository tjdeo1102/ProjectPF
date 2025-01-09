using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;

public class KSH_PlayerVoicesSetting : MonoBehaviourPun
{
    [SerializeField] private Speaker speaker;

    private void Start()
    {
        speaker = GetComponentInChildren<Speaker>();

        if(photonView.IsMine)
        {
            KSH_VoiceManager.Instance.AssignPlayerSpeaker(speaker);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

public class KSH_PlayerVoicesSetting : MonoBehaviour
{
    [SerializeField] private Speaker speaker;

    private void Awake()
    {
        speaker = GetComponent<Speaker>();
    }

    
}

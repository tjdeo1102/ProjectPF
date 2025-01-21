using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.PUN;

public class KSH_VoiceStatus : MonoBehaviour
{
    [SerializeField] private KSH_NetworkVoiceChat networkVoiceChat;
    [SerializeField] private Image voiceStatusImage;
    private PhotonVoiceView photonVoiceView;


    private void Awake()
    {
        // »óÀ§ °´Ã¼¿¡¼­ PhotonVoiceView¸¦ °¡Á®¿È
        this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
    }

    void Update()
    {
        voiceStatusImage.enabled = this.photonVoiceView.IsRecording;
    }
}
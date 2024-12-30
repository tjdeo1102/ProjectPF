using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_VoiceSingleton : MonoBehaviour
{
    void Awake()
    {
        // 이미 존재하는 PunVoiceClient가 있는 경우 현재 객체를 삭제
        if (FindObjectsOfType<PunVoiceClient>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // 중복이 아니라면 이 오브젝트를 유지
        DontDestroyOnLoad(gameObject);
    }
}

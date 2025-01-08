using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

public class KSH_PlayerVoicesSetting : MonoBehaviour
{
    [SerializeField] private Speaker speaker;
    [SerializeField] private KSH_VoicesVolumes voicesVolumes;

    private void Start()
    {
        speaker = GetComponentInChildren<Speaker>();
        // 태그가 "Voice"인 오브젝트를 찾습니다.
        GameObject enemyObject = GameObject.FindWithTag("Voice");

        // 찾은 오브젝트가 null이 아니면 KSH_VoicesVolumes 컴포넌트를 가져옵니다.
        if (enemyObject != null)
        {
            voicesVolumes = enemyObject.GetComponent<KSH_VoicesVolumes>();

            if (voicesVolumes == null)
            {
                Debug.LogError("컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("오브젝트를 찾을 수 없습니다.");
        }
        voicesVolumes.AssignPlayerSpeaker(speaker);
    }


}

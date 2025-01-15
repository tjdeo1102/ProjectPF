using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_AudioUI : MonoBehaviour
{
    // 슬라이더 참조
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    void Start()
    {
        // AudioManager에서 초기값 가져와 슬라이더에 설정
        masterVolumeSlider.value = KSH_PlayerData.PlayerSaveData.AudioVolume;
        bgmVolumeSlider.value = KSH_PlayerData.PlayerSaveData.BGMVolume;
        sfxVolumeSlider.value = KSH_PlayerData.PlayerSaveData.EffectVolume;

        // 슬라이더 값 변경 이벤트 등록
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        KSH_AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnBgmVolumeChanged(float value)
    {
        KSH_AudioManager.Instance.SetBgmVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        KSH_AudioManager.Instance.SetSfxVolume(value);
    }
}

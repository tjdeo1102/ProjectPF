using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_VoiceUI : MonoBehaviour
{
    [Header("마이크 볼륨")]
    [SerializeField] private float inputVolumes;

    [Header("음성 볼륨")]
    [SerializeField] private float outputVolumes;

    // 슬라이더 참조
    [SerializeField] private Slider inputVolumeSlider;
    [SerializeField] private Slider outputVolumeSlider;

    private void Start()
    {
        // 초기 값 설정
        inputVolumes = KSH_VoiceManager.Instance.InputVolume;
        outputVolumes = KSH_VoiceManager.Instance.OutputVolume;

        // 슬라이더 초기화
        inputVolumeSlider.value = inputVolumes;
        outputVolumeSlider.value = outputVolumes;

        // 슬라이더 값 변경 이벤트 등록
        inputVolumeSlider.onValueChanged.AddListener(SetInputVolume);
        outputVolumeSlider.onValueChanged.AddListener(SetOutputVolume);

        // VoiceManager 값 변경 이벤트 구독
        KSH_VoiceManager.Instance.OninputVolumeChanged += UpdateInputVolumeUI;
        KSH_VoiceManager.Instance.OnoutputVolumeChanged += UpdateOutputVolumeUI;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (KSH_VoiceManager.Instance != null)
        {
            KSH_VoiceManager.Instance.OninputVolumeChanged -= UpdateInputVolumeUI;
            KSH_VoiceManager.Instance.OnoutputVolumeChanged -= UpdateOutputVolumeUI;
        }

        // 슬라이더 이벤트 해제
        inputVolumeSlider.onValueChanged.RemoveListener(SetInputVolume);
        outputVolumeSlider.onValueChanged.RemoveListener(SetOutputVolume);
    }

    // 슬라이더 값 변경 시 호출
    private void SetInputVolume(float value)
    {
        KSH_VoiceManager.Instance.InputVolume = value; // VoiceManager에 반영
    }

    private void SetOutputVolume(float value)
    {
        KSH_VoiceManager.Instance.OutputVolume = value; // VoiceManager에 반영
    }

    // VoiceManager에서 값이 변경될 때 UI 업데이트
    private void UpdateInputVolumeUI(float newInputVolume)
    {
        inputVolumes = newInputVolume; // 내부 값 업데이트
        inputVolumeSlider.value = newInputVolume; // 슬라이더 값 업데이트
    }

    private void UpdateOutputVolumeUI(float newOutputVolume)
    {
        outputVolumes = newOutputVolume; // 내부 값 업데이트
        outputVolumeSlider.value = newOutputVolume; // 슬라이더 값 업데이트
    }
}

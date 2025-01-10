using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Collections;
using UnityEngine;

public class KSH_VoiceManager : MonoBehaviour
{
    public static KSH_VoiceManager Instance;

    // 마이크 입력용 Recorder
    [SerializeField] private Recorder recorder;

    // 수신된 음성 출력용 기본 Speaker (프리팹에 할당된 Speaker)
    [SerializeField] private Speaker speaker1;

    // 동적으로 할당될 플레이어 Speaker
    [SerializeField] private Speaker speaker2;

    // UI용 이벤트 등록
    public Action<float> OninputVolumeChanged;
    public Action<float> OnoutputVolumeChanged;

    [Range(0f, 1f)]
    [SerializeField] private float inputVolume = 1f;  // 입력 음량 (0.0 ~ 1.0)
    [Range(0f, 1f)]
    [SerializeField] private float outputVolume = 1f; // 출력 음량 (0.0 ~ 1.0)

    // Speaker와 연결된 AudioSource
    private AudioSource audioSource;
    // 현재 활성화된 Speaker 확인
    private Speaker currentSpeaker;

    private void Awake()
    {
        // Singleton 패턴 적용
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 이미 존재하는 PunVoiceClient가 있는 경우 현재 객체를 삭제
        if (FindObjectsOfType<PunVoiceClient>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // 중복이 아니라면 이 오브젝트를 유지
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Recorder 초기화
        if (recorder == null)
        {
            recorder = GetComponent<Recorder>();
        }

        // 사용자 정의 프로세서 추가
        if (recorder != null)
        {
            recorder.UserData = new VolumeProcessor(inputVolume);
        }

        // Speaker 초기화 및 AudioSource 가져오기
        if (speaker1 == null)
        {
            speaker1 = GetComponent<Speaker>();
        }

        // Speaker 초기화
        InitializeSpeaker(speaker1); // 기본 Speaker로 설정
    }

    public float InputVolume
    {
        get => inputVolume;
        set
        {
            if (inputVolume != value)
            {
                inputVolume = value;
                OninputVolumeChanged?.Invoke(inputVolume); // 값 변경 이벤트 호출
            }
        }
    }

    public float OutputVolume
    {
        get => outputVolume;
        set
        {
            if (outputVolume != value)
            {
                outputVolume = value;
                OnoutputVolumeChanged?.Invoke(outputVolume); // 값 변경 이벤트 호출
            }
        }
    }

    private void Update()
    {
        // speaker2가 null인 경우 speaker1 활성화
        if (speaker2 == null && currentSpeaker != speaker1)
        {
            InitializeSpeaker(speaker1);
        }

        // 입력 볼륨 업데이트
        if (recorder != null && recorder.UserData is VolumeProcessor volumeProcessor)
        {
            if (Mathf.Abs(volumeProcessor.GetVolumeMultiplier() - inputVolume) > Mathf.Epsilon)
            {
                volumeProcessor.SetVolumeMultiplier(inputVolume); // 변경된 경우에만 업데이트
            }
        }

        // 출력 볼륨 업데이트
        if (audioSource != null && Mathf.Abs(audioSource.volume - outputVolume) > Mathf.Epsilon)
        {
            audioSource.volume = outputVolume; // 변경된 경우에만 업데이트
        }
    }

    private void InitializeSpeaker(Speaker speaker)
    {
        if (speaker != null)
        {
            // 현재 활성화된 Speaker를 업데이트
            currentSpeaker = speaker;
            audioSource = speaker.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.volume = outputVolume; // 초기 볼륨 설정
            }

            Debug.Log($"활성화된 Speaker: {speaker.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Speaker가 null입니다.");
        }
    }

    public void AssignPlayerSpeaker(Speaker playerSpeaker)
    {
        if (playerSpeaker != null)
        {
            speaker2 = playerSpeaker; // speaker2에 새 Speaker 할당
            InitializeSpeaker(speaker2); // speaker2를 활성화된 Speaker로 설정
        }
        else
        {
            Debug.LogWarning("Speaker2가 null입니다.");
        }
    }
}

// 사용자 정의 IAudioProcessor 구현
public class VolumeProcessor : IProcessor<float>
{
    // 볼륨 배율
    private float volumeMultiplier;

    // 볼륨 배율 초기값 설정
    public VolumeProcessor(float volumeMultiplier)
    {
        this.volumeMultiplier = volumeMultiplier;
    }

    // 볼륨 배율 값을 업데이트하는 함수
    public void SetVolumeMultiplier(float newMultiplier)
    {
        volumeMultiplier = newMultiplier;
    }

    // 현재 볼륨 배율 반환
    public float GetVolumeMultiplier()
    {
        return volumeMultiplier;
    }

    public float[] Process(float[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] *= volumeMultiplier; // 볼륨 배율 적용
        }
        return buffer;
    }

    public void Dispose()
    {
        // 리소스 정리 (필요시 구현)
    }
}
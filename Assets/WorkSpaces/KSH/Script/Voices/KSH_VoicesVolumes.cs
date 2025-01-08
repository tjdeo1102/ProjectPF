using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class KSH_VoicesVolumes : MonoBehaviour
{
    // 마이크 입력용 Recorder
    [SerializeField] private Recorder recorder;

    // 수신된 음성 출력용 기본 Speaker (프리팹에 할당된 Speaker)
    [SerializeField] private Speaker speaker1;

    // 동적으로 할당될 플레이어 Speaker
    [SerializeField] private Speaker speaker2;

    [Range(0f, 1f)]
    [SerializeField] private float inputVolume = 1f;  // 입력 음량 (0.0 ~ 1.0)
    [Range(0f, 1f)]
    [SerializeField] private float outputVolume = 1f; // 출력 음량 (0.0 ~ 1.0)

    // Speaker와 연결된 AudioSource
    private AudioSource audioSource;
    // 현재 활성화된 Speaker 확인
    private Speaker currentSpeaker;

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
        InitializeSpeaker(speaker1); // speaker1을 기본으로 설정
    }

    private void Update()
    {
        if (speaker2 == null)
        {
            if (currentSpeaker != speaker1)
            {
                InitializeSpeaker(speaker1); // speaker1을 다시 활성화
            }
        }
        // 입력 음량 조절
        if (recorder != null && recorder.UserData is VolumeProcessor volumeProcessor)
        {
            // 볼륨 실시간 업데이트
            volumeProcessor.SetVolumeMultiplier(inputVolume);
        }

        // 출력 음량 조절
        if (audioSource != null)
        {
            audioSource.volume = outputVolume;
        }
    }

    private void InitializeSpeaker(Speaker speaker)
    {
        if (speaker != null)
        {
            // 현재 활성화된 Speaker를 업데이트
            currentSpeaker = speaker;
            audioSource = speaker.GetComponent<AudioSource>();
            Debug.Log($"활성화된 Speaker: {speaker.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Speaker1가 null입니다.");
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

    // 볼륨 배율 값을 업데이트 하는 함수
    public void SetVolumeMultiplier(float newMultiplier)
    {
        volumeMultiplier = newMultiplier;
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
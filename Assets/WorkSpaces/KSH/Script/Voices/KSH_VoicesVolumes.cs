using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class KSH_VoicesVolumes : MonoBehaviour
{
    [SerializeField] private Recorder recorder; // 마이크 입력용 Recorder
    [SerializeField] private Speaker speaker;   // 수신된 음성 출력용 Speaker

    [Range(0f, 1f)]
    [SerializeField] private float inputVolume = 1f;  // 입력 음량 (0.0 ~ 1.0)
    [Range(0f, 1f)]
    [SerializeField] private float outputVolume = 1f; // 출력 음량 (0.0 ~ 1.0)

    private AudioSource audioSource; // Speaker와 연결된 AudioSource

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
        if (speaker == null)
        {
            speaker = GetComponent<Speaker>();
        }

        if (speaker != null)
        {
            audioSource = speaker.GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 입력 음량 조절
        if (recorder != null && recorder.UserData is VolumeProcessor volumeProcessor)
        {
            volumeProcessor.SetVolumeMultiplier(inputVolume); // 볼륨 실시간 업데이트
        }

        // 출력 음량 조절
        if (audioSource != null)
        {
            audioSource.volume = outputVolume;
        }
    }
}

// 사용자 정의 IAudioProcessor 구현
public class VolumeProcessor : IProcessor<float>
{
    private float volumeMultiplier;

    public VolumeProcessor(float volumeMultiplier)
    {
        this.volumeMultiplier = volumeMultiplier;
    }

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
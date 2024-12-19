using UnityEngine;

public class KSH_AudioManager : MonoBehaviour
{
    public static KSH_AudioManager Instance;

    [Header("BGM")]
    public AudioClip[] bgmClips; // 배경음악 클립 배열
    public float bgmVolume;        // 배경음악 볼륨
    AudioSource bgmPlayer;         // 배경음악을 재생하는 오디오 소스

    [Header("SFX")]
    public AudioClip[] sfxClips; // 효과음 클립 배열
    public bool[] sfxLoops;      // 효과음마다 반복 여부 설정 배열
    public float sfxVolume;       // 효과음 볼륨
    public int channels;          // 동시 재생할 효과음 채널 수
    AudioSource[] sfxPlayers;    // 효과음 재생을 위한 오디오 소스 배열
    int channelIndex;            // 현재 사용 중인 채널 인덱스

    public enum Sfx
    {
        Slice
    } // 예시 효과음 종류 설정

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        // 배경음악 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        //bgmPlayer.volume = bgmVolume;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            //sfxPlayers[index].volume = sfxVolume;
        }
    }

    // 배경음악 재생 메소드
    public void PlayBgm(int bgmIndex)
    {
        if (bgmIndex >= 0 && bgmIndex < bgmClips.Length)
        {
            bgmPlayer.clip = bgmClips[bgmIndex]; // 선택한 배경음악 클립 설정
            bgmPlayer.Play(); // 배경음악 재생
        }
    }

    public void StopBgm()
    {
        bgmPlayer.Stop(); // 배경음악 중지
    }

    // 효과음 재생 메소드
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].loop = sfxLoops[(int)sfx]; // loop 설정 적용
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    // 모든 효과음 중지 메소드
    public void StopSfx()
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i].isPlaying)
            {
                sfxPlayers[i].Stop(); // 재생 중인 효과음 중지
            }
        }
    }

    // 배경음악 볼륨 조절 메소드
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f); // 볼륨을 0에서 1 사이로 제한
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = bgmVolume; // BGM 플레이어 볼륨 설정
        }
    }

    // 효과음 볼륨 조절 메소드
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 1f); // 볼륨을 0에서 1 사이로 제한
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i] != null)
            {
                sfxPlayers[i].volume = sfxVolume; // 각 SFX 플레이어 볼륨 설정
            }
        }
    }
}



// 사용 방법
// AudioManager.Instance.PlayBgm(0); 첫 번째 배경음악 재생 0대신 1입력 시 두번째 배경음악
// AudioManager.Instance.StopBgm();  배경 음악 멈출 때 사용

// 효과음
// AudioManager.Instance.PlaySfx(AudioManager.Sfx.Clicks); 선택한 효과음 재생 및 종료

// 배경음 볼륨을 50%로 설정
// AudioManager.Instance.SetBgmVolume(0.5f);

// SFX 볼륨을 70%로 설정
// AudioManager.Instance.SetSfxVolume(0.7f);
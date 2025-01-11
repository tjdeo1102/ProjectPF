using UnityEngine;
using UnityEngine.Audio;

public class KSH_AudioManager : MonoBehaviour
{
    public static KSH_AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;          // 마스터 mixer 참조
    public AudioMixerGroup bgmMixer;  // BGM용 Mixer Group
    public AudioMixerGroup sfxMixer;  // SFX용 Mixer Group

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
        Button1, Button2, Button3, Button4, Button5, Pick_fruit, Pick_scoop, Pick_clamp, Cut1, Knife, Cut2, 
        Blender_in, Blender_Lid1, Blender_Lid2, Blender_handle, Blender_play, Blender_out, Mortar_in, Mortar_play,
        Mortar_end, Mortar_out, Cauldron_in1, Cauldron_in2, Cauldron_change, Cauldron_fall,
        Cauldron_coal, Pick_coal, Cauldron_fire1, Cauldron_fire2, Cauldron_fire3, Cauldron_boils, Cauldron_water,
        Cauldron_clear, Cauldron_out, Tank_out, Tank_button, Dispenser_in, Dispenser_out, Perfume_success,
        Perfume_fall, Guest_wait, Test1, Test2, Guest_feedback3, Guest_feedback2, Guest_feedback1, Guest_feedback0,
        Pick_Bottle, Label_sign, Label_on, Guest_success, Guest_fall, Bill1, Bill2, Tablet_on, Tablet_click,
        Tablet_cansel, Tablet_success, Tablet_fall, Furniture_on, Next_Stage
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

    void Init()             // outputAudioMixerGroup 특정 AudioMixerGroup 연결 할때 사용
    {
        // 배경음악 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.outputAudioMixerGroup = bgmMixer; // BGM Mixer Group 설정
        //bgmPlayer.volume = bgmVolume;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].outputAudioMixerGroup = sfxMixer; // SFX Mixer Group 설정
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

    // 반복 효과음 중지 메소드
    public void StopSfxLoop(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i].clip == sfxClips[(int)sfx] && sfxPlayers[i].isPlaying)
            {
                sfxPlayers[i].loop = false; // 반복 재생 비활성화
                sfxPlayers[i].Stop(); // 재생 중지
                sfxPlayers[i].loop = true; // 반복 설정 복원
            }
        }
    }

    // AudioMixer를 통한 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20; // dB로 변환
        audioMixer.SetFloat("MasterVolume", dB);
    }

    public void SetBgmVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSfxVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat("SFXVolume", dB);
    }

    // AudioMixer에서 볼륨 가져오기
    public float GetMasterVolume()
    {
        audioMixer.GetFloat("MasterVolume", out float value);
        return value;
    }

    public float GetBgmVolume()
    {
        audioMixer.GetFloat("BGMVolume", out float value);
        return value;
    }

    public float GetSfxVolume()
    {
        audioMixer.GetFloat("SFXVolume", out float value);
        return value;
    }
}



// 사용 방법
// KSH_AudioManager.Instance.PlayBgm(0); 첫 번째 배경음악 재생 0대신 1입력 시 두번째 배경음악
// KSH_AudioManager.Instance.StopBgm();  배경 음악 멈출 때 사용

// 효과음
// KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Clicks); 선택한 효과음 재생 및 종료

// 배경음 볼륨을 50%로 설정
// KSH_AudioManager.Instance.SetBgmVolume(0.5f);

// SFX 볼륨을 70%로 설정
// KSH_AudioManager.Instance.SetSfxVolume(0.7f);
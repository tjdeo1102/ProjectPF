using UnityEngine;

public class KSH_AudioManager : MonoBehaviour
{
    public static KSH_AudioManager Instance;

    [Header("BGM")]
    public AudioClip[] bgmClips; // ������� Ŭ�� �迭
    public float bgmVolume;        // ������� ����
    AudioSource bgmPlayer;         // ��������� ����ϴ� ����� �ҽ�

    [Header("SFX")]
    public AudioClip[] sfxClips; // ȿ���� Ŭ�� �迭
    public bool[] sfxLoops;      // ȿ�������� �ݺ� ���� ���� �迭
    public float sfxVolume;       // ȿ���� ����
    public int channels;          // ���� ����� ȿ���� ä�� ��
    AudioSource[] sfxPlayers;    // ȿ���� ����� ���� ����� �ҽ� �迭
    int channelIndex;            // ���� ��� ���� ä�� �ε���

    public enum Sfx
    {
        Slice
    } // ���� ȿ���� ���� ����

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
        // ������� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        //bgmPlayer.volume = bgmVolume;

        // ȿ���� �÷��̾� �ʱ�ȭ
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

    // ������� ��� �޼ҵ�
    public void PlayBgm(int bgmIndex)
    {
        if (bgmIndex >= 0 && bgmIndex < bgmClips.Length)
        {
            bgmPlayer.clip = bgmClips[bgmIndex]; // ������ ������� Ŭ�� ����
            bgmPlayer.Play(); // ������� ���
        }
    }

    public void StopBgm()
    {
        bgmPlayer.Stop(); // ������� ����
    }

    // ȿ���� ��� �޼ҵ�
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
            sfxPlayers[loopIndex].loop = sfxLoops[(int)sfx]; // loop ���� ����
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    // ��� ȿ���� ���� �޼ҵ�
    public void StopSfx()
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i].isPlaying)
            {
                sfxPlayers[i].Stop(); // ��� ���� ȿ���� ����
            }
        }
    }

    // ������� ���� ���� �޼ҵ�
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f); // ������ 0���� 1 ���̷� ����
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = bgmVolume; // BGM �÷��̾� ���� ����
        }
    }

    // ȿ���� ���� ���� �޼ҵ�
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 1f); // ������ 0���� 1 ���̷� ����
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i] != null)
            {
                sfxPlayers[i].volume = sfxVolume; // �� SFX �÷��̾� ���� ����
            }
        }
    }
}



// ��� ���
// AudioManager.Instance.PlayBgm(0); ù ��° ������� ��� 0��� 1�Է� �� �ι�° �������
// AudioManager.Instance.StopBgm();  ��� ���� ���� �� ���

// ȿ����
// AudioManager.Instance.PlaySfx(AudioManager.Sfx.Clicks); ������ ȿ���� ��� �� ����

// ����� ������ 50%�� ����
// AudioManager.Instance.SetBgmVolume(0.5f);

// SFX ������ 70%�� ����
// AudioManager.Instance.SetSfxVolume(0.7f);
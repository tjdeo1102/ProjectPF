using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_LobbyBGM : MonoBehaviour
{
    private void Start()
    {
        KSH_AudioManager.Instance.PlayBgm(0);
    }

    public void StopBGM()
    {
        KSH_AudioManager.Instance.StopBgm();
    }
}

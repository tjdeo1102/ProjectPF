using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_TestAudio : MonoBehaviour
{
    private void Start()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            KSH_AudioManager.Instance.StopSfxLoop(KSH_AudioManager.Sfx.Button1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
        }
    }
}

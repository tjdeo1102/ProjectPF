using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_TestAudio : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
            KSH_EffectManager.Instance.PlayEffect(KSH_EffectManager.Effect.Fire_small, transform.position);
        }
    }
}

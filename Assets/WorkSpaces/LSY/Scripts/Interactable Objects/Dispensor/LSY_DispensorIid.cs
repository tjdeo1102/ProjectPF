using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DispensorIid : MonoBehaviour
{
    public LSY_DispensorLiquid dispensorLiquid;

    public void Open()
    {
        if (dispensorLiquid.isLitOpen == false)
        {
            OpenSound();
            dispensorLiquid.LitAnimation("LidOn", true);

        }
        else
        {
            CloseSound();
            dispensorLiquid.LitAnimation("LidOff", false);
            dispensorLiquid.LitAnimation("LidIdle", false);

        }
    }

    public void OpenSound()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.OpenDispenser);
    }

    public void CloseSound()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.CloseDispenser);
    }
}

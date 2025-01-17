using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_ButtonClick : MonoBehaviour
{
    public void ButtonClick()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
    }
}

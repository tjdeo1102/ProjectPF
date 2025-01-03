using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WGH_CheatButton : MonoBehaviour
{
    public void Cheat()
    {
        WGH_NPCCreator.Instance.OnCheat();
    }
}

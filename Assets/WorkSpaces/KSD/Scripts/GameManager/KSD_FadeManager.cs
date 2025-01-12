using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_FadeManager : MonoBehaviour
{
    public float fadeDuration;
    public List<KSD_FadeObject> fadeObjects;
    private void Awake()
    {
        fadeObjects = new List<KSD_FadeObject>();
        // FadeIn은 각자의 KSD_FadeObject에서 담당
    }

    public void FadeOut()
    {
        for (int i = 0; i < fadeObjects.Count; i++)
        {
            fadeObjects[i].SetFade(fadeDuration, 1);
        }
    }
}

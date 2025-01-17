using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KSD_FadeObject : MonoBehaviour
{
    public Renderer fadeRenderer;

    private float playDuration;
    private float targetAlpha;

    private void Start()
    {
        KSD_GameManager.Instance.fadeManager.fadeObjects.Add(this);
        SetFade(KSD_GameManager.Instance.fadeManager.fadeDuration, 0);
    }

    public void SetFade(float duration, float alpha)
    {
        if (fadeRenderer == null || fadeRenderer.material == null) return;
        playDuration = duration;
        targetAlpha = alpha;
        
        var originCol = fadeRenderer.material.color;
        fadeRenderer.material.DOColor(new Color(originCol.r, originCol.g, originCol.b, targetAlpha), playDuration);
    }

}

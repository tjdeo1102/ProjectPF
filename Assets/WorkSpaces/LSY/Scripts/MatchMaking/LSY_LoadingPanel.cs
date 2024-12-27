using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LSY_LoadingPanel : MonoBehaviour
{
    [SerializeField] TMP_Text loadingText;
    [SerializeField] Scrollbar scrollbar;

    public void Start()
    {
        StartCoroutine(LoadingTextRoutine());
        StartCoroutine(LoadingBarRoutine());
        Destroy(gameObject, 3f);
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator LoadingTextRoutine()
    {
        while (true)
        {
            loadingText.text = "LOADING.";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "LOADING..";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "LOADING...";
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator LoadingBarRoutine()
    {
        float duration = 3f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; 
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration); 
            scrollbar.size = normalizedTime; 

            yield return null;
        }

        scrollbar.size = 1f;
    }
}

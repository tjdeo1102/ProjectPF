using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KSD_CauldronUI : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;      
    [SerializeField] private float displayTime = 3f; // 텍스트가 표시될 시간

    private Coroutine hideTextCoroutine;

    // 텍스트 설정 함수
    public void SetText(string message, Color col)
    {
        // 기존 코루틴이 실행 중이면 중단
        if (hideTextCoroutine != null)
        {
            StopCoroutine(hideTextCoroutine);
        }

        uiText.text = message;
        uiText.enabled = true;
        uiText.color = col;

        hideTextCoroutine = StartCoroutine(HideTextAfterDelay());
    }

    private IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        uiText.enabled = false;
        hideTextCoroutine = null;
    }
}

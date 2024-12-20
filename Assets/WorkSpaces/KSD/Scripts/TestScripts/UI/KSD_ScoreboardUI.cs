using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KSD_ScoreboardUI : MonoBehaviour
{
    [Header("기본 UI 요소 설정")]
    [SerializeField] private TMP_Text stageIDText;
    [SerializeField] private TMP_Text stageLevelText;
    [SerializeField] private TMP_Text finishCountText;
    void Start()
    {
        KSD_GameManager.Instance.OnChangeStageInfo.AddListener(UpdateUI);
    }

    private void OnDisable()
    {
        KSD_GameManager.Instance.OnChangeStageInfo.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        var info = KSD_GameManager.Instance.CurrentStageInfo;
        stageIDText?.SetText(info.StageID.ToString());
        stageLevelText?.SetText(info.StageLevel.ToString());
        finishCountText?.SetText(info.FinishPlayerCount.ToString());
    }    
}

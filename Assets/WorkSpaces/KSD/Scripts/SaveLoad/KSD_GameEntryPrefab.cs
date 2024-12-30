using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSD_GameEntryPrefab : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private KSD_StageInfo stageInfo;
    public KSD_StageInfo StageInfo
    {
        get { return stageInfo; }
        set { stageInfo = value; }
    }
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;
    private LSY_RoomPanel roomPanel;

    public void UpdateStageInfo()
    {
        if (text == null) return;
        if (StageInfo.StageID == -1)
        {
            text.SetText("None");
        }
        else
        {
            if (DateTime.TryParse(stageInfo.StageDate,out var resTime))
            {
                text.SetText($"{resTime.Month}.{resTime.Day} / Day - {stageInfo.StageLevel}");
            }
            else 
            {
                text.SetText($"Missing / Day - {stageInfo.StageLevel}");
            }
        }
    }

    public void OnClickButtonBind(LSY_RoomPanel roomPanel)
    {
        if (roomPanel == null) return;
        this.roomPanel = roomPanel;
        button.onClick.AddListener(OnClickButton);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        roomPanel.SetStageInfo(StageInfo);
        roomPanel.StartGame();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class KSD_ReceiptController : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text visitCountText;
    [SerializeField] private TMP_Text saleCountText;
    [SerializeField] private TMP_Text IncomeText;
    [SerializeField] private TMP_Text endTimeText;
    [SerializeField] private Button nextButton;

    private float startTime;
    private int startMoney;

    private void Start()
    {
        KSD_GameManager.Instance.OnCanExitStage.AddListener(OnExitStageHandler);
        KSD_GameManager.Instance.OnChangeStageInfo.AddListener(Init);
    }

    private void OnDisable()
    {
        KSD_GameManager.Instance.OnCanExitStage.RemoveListener(OnExitStageHandler);
    }

    public void Init()
    {
        KSD_GameManager.Instance.OnChangeStageInfo.RemoveListener(Init);

        startMoney = KSD_GameManager.Instance.CurrentStageInfo.StageMoney;
        startTime = Time.time;
    }

    public void OnExitStageHandler()
    {
        var info = KSD_GameManager.Instance.CurrentStageInfo;
        dayText.SetText($"{info.StageLevel}ÀÏÂ÷");
        visitCountText.SetText($"{info.VisitPlayerCount}");
        saleCountText.SetText($"{info.FinishPlayerCount}");
        IncomeText.SetText($"{info.StageMoney - startMoney}");
        endTimeText.SetText($"{Time.time - startMoney}");

    }
}

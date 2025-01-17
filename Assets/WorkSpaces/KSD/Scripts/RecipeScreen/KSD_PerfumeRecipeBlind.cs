using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CauldronContent;

public class KSD_PerfumeRecipeBlind : MonoBehaviour
{
    [SerializeField] private GameObject[] recipes;

    private void Start()
    {
        KSD_GameManager.Instance.OnChangeStageInfo.AddListener(UpdateRecipe);
    }

    private void OnDisable()
    {
        KSD_GameManager.Instance.OnChangeStageInfo.RemoveListener(UpdateRecipe);
    }

    public void UpdateRecipe()
    {
        // 매니저의 스테이지 정보를 통해 레시피 여부 업데이트
        var list = KSD_GameManager.Instance.CurrentStageInfo.ActivePerfumes;
        for (int i = 1; i < list.Count; i++)
        {
            // 1부터 향수 이름 시작 (0: NULL)
            recipes[i-1].SetActive(!list[i]);
        }
    }
}

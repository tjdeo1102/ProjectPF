using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_NoteRecipeBlind : MonoBehaviour
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
        var list = KSD_GameManager.Instance.CurrentStageInfo.ActiveNotes;
        var cnt = list.Count - 2;
        for (int i = 0; i < cnt; i++)
        {
            recipes[i].SetActive(!list[i]);
        }

        // 또한, 스테이지 레벨에 따라 해당 스테이지와 같은 레벨의 노트 레시피 개방
        // 스테이지 레벨 - 1 = 향수 인덱스
        var idx = KSD_GameManager.Instance.CurrentStageInfo.StageLevel - 1;
        recipes[idx].SetActive(false);
    }
}

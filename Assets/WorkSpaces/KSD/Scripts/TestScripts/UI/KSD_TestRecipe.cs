using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class KSD_TestRecipe : MonoBehaviour
{
    [SerializeField] private List<KSD_PerfumeMaterialInfo> perfumeMatInfos;
    [SerializeField] private List<KSD_PerfumeNoteInfo> perfumeConcentrateInfos;
    [SerializeField] private List<KSD_PerfumeNoteInfo> perfumeNoteInfos;


    public void MakeConcentrate()
    {
        var res = KSD_RecipeManager.Instance.IsValidConcentrateRecipe(perfumeMatInfos);
        Debug.Log($"원료 조합 성공 여부: {res}");
    }

    public void MakeNote()
    {
        var res = KSD_RecipeManager.Instance.IsValidNoteRecipe(perfumeConcentrateInfos);
        Debug.Log($"노트 조합 성공 여부: {res}");
    }

    public void MakePerfume()
    {
        var res = KSD_RecipeManager.Instance.IsValidPerfumeRecipe(perfumeNoteInfos);
        Debug.Log($"향수 조합 성공 여부: {res}");
    }
}

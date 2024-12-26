using Photon.Pun;
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
        var res = KSD_PerfumeManager.Instance.IsValidConcentrateRecipe(perfumeMatInfos, out var a);
        Debug.Log($"원료 조합 성공 여부: {res} \n 원료 정보 {a}");
    }

    public void MakeNote()
    {
        var res = KSD_PerfumeManager.Instance.IsValidNoteRecipe(perfumeConcentrateInfos, out var a);
        Debug.Log($"노트 조합 성공 여부: {res} \n 노트 정보 {a}");
    }

    public void MakePerfume()
    {
        var res = KSD_PerfumeManager.Instance.IsValidPerfumeRecipe(perfumeNoteInfos, out var a);
        Debug.Log($"향수 조합 성공 여부: {res} \n 향수 정보 {a}");
    }
}

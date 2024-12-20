using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_WGH_PerfumeType
{
    SPICY,
    COOL,
    COMFORTABLE,
    E_PERFUMETYPE_MAX
}
public class WGH_Perfume : MonoBehaviour
{
    public E_WGH_PerfumeType PerfumeType;
    public E_WGH_PerfumeMaterialType BestMaterial;               // 최고 재료
    public E_WGH_PerfumeMaterialType LikeMaterial;               // 선호 재료
    public E_WGH_PerfumeMaterialType QuestionMaterial;           // 의문 재료

    private void Start()
    {
        SelectMaterial();
    }

    private void SelectMaterial()
    {
        if (PerfumeType == E_WGH_PerfumeType.SPICY)
        {
            BestMaterial = E_WGH_PerfumeMaterialType.SPICY;
            LikeMaterial = E_WGH_PerfumeMaterialType.HOT;
            QuestionMaterial = E_WGH_PerfumeMaterialType.None;
        }
        else if(PerfumeType == E_WGH_PerfumeType.COOL)
        {
            BestMaterial = E_WGH_PerfumeMaterialType.COOL;
            LikeMaterial = E_WGH_PerfumeMaterialType.COMFORTABLE;
            QuestionMaterial = E_WGH_PerfumeMaterialType.None;
        }
        else if(PerfumeType == E_WGH_PerfumeType.COMFORTABLE) 
        {
            BestMaterial = E_WGH_PerfumeMaterialType.COMFORTABLE;
            LikeMaterial = E_WGH_PerfumeMaterialType.HOT;
            QuestionMaterial = E_WGH_PerfumeMaterialType.None;
        }
    }
}

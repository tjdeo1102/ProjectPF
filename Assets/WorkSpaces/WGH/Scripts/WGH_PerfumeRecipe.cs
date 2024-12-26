using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_WGH_PerfumeType
{
    NONE = 0,
    RECIPE1,
    RECIPE2,
    RECIPE3,
    RECIPE4,
    RECIPE5,
    RECIPE6,
    RECIPE7,
    RECIPE8,
    RECIPE9,   
    E_PERFUMETYPE_MAX
}

public enum E_WGH_NoteType
{
    NONE = 0,
    FLORAL,
    FRUITY,
    CITRUS,
    WOODY,
    GREEN,
    E_WGH_PERFUMEMATERIAL_MAX
}

public enum E_BottleType // 플로팅 되는 병 이미지
{
    a,
    b,
    c,
    d,
    e,
    E_BottleType_MAX
}

public class WGH_PerfumeRecipe : MonoBehaviour
{
    public E_WGH_PerfumeType PerfumeType;               // 향수별로 미리 설정 필요
    public E_BottleType BottleType;                     // 병 타입 미리 설정 필요

    public E_WGH_NoteType BestMaterial;               // 최고 재료
    public E_WGH_NoteType LikeMaterial;               // 선호 재료
    public E_WGH_NoteType LikeMaterial2;               // 선호 재료2
    public E_WGH_NoteType QuestionMaterial;           // 의문 재료
    public E_WGH_NoteType QuestionMaterial2;           // 의문 재료2

    private void Start()
    {
        SelectPerfumeType();
    }

    private void SelectPerfumeType()
    {
        switch((int)PerfumeType)
        {
            case 1:
                BestMaterial = E_WGH_NoteType.FLORAL;
                LikeMaterial = E_WGH_NoteType.NONE;
                LikeMaterial2 = E_WGH_NoteType.NONE;
                QuestionMaterial = E_WGH_NoteType.CITRUS;
                QuestionMaterial2 = E_WGH_NoteType.GREEN;
                break;
            case 2:
                BestMaterial = E_WGH_NoteType.WOODY;
                LikeMaterial = E_WGH_NoteType.GREEN;
                LikeMaterial2 = E_WGH_NoteType.NONE;
                QuestionMaterial = E_WGH_NoteType.NONE;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 3:
                BestMaterial = E_WGH_NoteType.FRUITY;
                LikeMaterial = E_WGH_NoteType.NONE;
                LikeMaterial2 = E_WGH_NoteType.NONE;
                QuestionMaterial = E_WGH_NoteType.CITRUS;
                QuestionMaterial2 = E_WGH_NoteType.GREEN;
                break;
            case 4:
                BestMaterial = E_WGH_NoteType.GREEN;
                LikeMaterial = E_WGH_NoteType.NONE;
                LikeMaterial2 = E_WGH_NoteType.NONE;
                QuestionMaterial = E_WGH_NoteType.WOODY;
                QuestionMaterial2 = E_WGH_NoteType.FLORAL;
                break;
            case 5:
                BestMaterial = E_WGH_NoteType.CITRUS;
                LikeMaterial = E_WGH_NoteType.GREEN;
                LikeMaterial2 = E_WGH_NoteType.NONE;
                QuestionMaterial = E_WGH_NoteType.NONE;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 6:
                BestMaterial = E_WGH_NoteType.NONE;
                LikeMaterial = E_WGH_NoteType.WOODY;
                LikeMaterial2 = E_WGH_NoteType.FLORAL;
                QuestionMaterial = E_WGH_NoteType.CITRUS;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 7:
                BestMaterial = E_WGH_NoteType.NONE;
                LikeMaterial = E_WGH_NoteType.FRUITY;
                LikeMaterial2 = E_WGH_NoteType.FLORAL;
                QuestionMaterial = E_WGH_NoteType.GREEN;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 8:
                BestMaterial = E_WGH_NoteType.NONE;
                LikeMaterial = E_WGH_NoteType.FRUITY;
                LikeMaterial2 = E_WGH_NoteType.CITRUS;
                QuestionMaterial = E_WGH_NoteType.FLORAL;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 9:
                BestMaterial = E_WGH_NoteType.NONE;
                LikeMaterial = E_WGH_NoteType.WOODY;
                LikeMaterial2 = E_WGH_NoteType.GREEN;
                QuestionMaterial = E_WGH_NoteType.FRUITY;
                QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
        }
    }
}

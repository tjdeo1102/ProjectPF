using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WGH_NPCNote : MonoBehaviour
{
    public E_WGH_PerfumeType PerfumeType;
    public E_WGH_NoteType BestMaterial;
    public E_WGH_NoteType LikeMaterial;
    public E_WGH_NoteType LikeMaterial2;
    public E_WGH_NoteType QuestionMaterial;
    public E_WGH_NoteType QuestionMaterial2;

    private void Awake()
    {
        SelectPerfumeType();
    }

    private void SelectPerfumeType()
    {
        int randNum = Random.Range(1, (int)E_WGH_PerfumeType.E_PERFUMETYPE_MAX - 1);
        PerfumeType = (E_WGH_PerfumeType)randNum;

        switch (randNum)
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perfume Recipe Data", menuName = "Scriptable Object/Perfume Recipe")]
public class KSD_PerfumeRecipe:ScriptableObject
{
    [Header("필요 재료")]
    public List<KSD_PerfumeNoteInfo> NeedNotes;

    [Header("결과물")]
    public KSD_PerfumeInfo ResultPerfume;

    public bool isPrivate;  // 레시피 공개 여부
}

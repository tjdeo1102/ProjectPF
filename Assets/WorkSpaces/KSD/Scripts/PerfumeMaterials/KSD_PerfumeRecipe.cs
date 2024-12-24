using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perfume Recipe Data", menuName = "Scriptable Object/Perfume Recipe")]
public class KSD_PerfumeRecipe:ScriptableObject
{
    public List<KSD_PerfumeNoteInfo> NeedNotes;
    public bool isPrivate;  // 레시피 공개 여부
}

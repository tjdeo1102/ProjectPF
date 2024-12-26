using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Concentrate Recipe Data", menuName = "Scriptable Object/Concentrate Recipe")]
public class KSD_ConcentrateRecipe:ScriptableObject
{
    [Header("필요 재료")]
    public List<KSD_PerfumeMaterialInfo> NeedMaterials;

    [Header("결과물")]
    public KSD_PerfumeNoteInfo ResultConcentrate;

    private void OnValidate()
    {
        if (NeedMaterials != null &&
            NeedMaterials.Count != 2)
        {
            Debug.Log($"{nameof(NeedMaterials)} 개수는 2개 고정");
            int count = NeedMaterials.Count;

            if (NeedMaterials.Count > 2) NeedMaterials.RemoveRange(2, count - 2);
            else
            {
                while (NeedMaterials.Count != 2)
                {
                    NeedMaterials.Add(null);
                }
            }
        }
    }
}

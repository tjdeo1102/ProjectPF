using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Concentrate Recipe Data", menuName = "Scriptable Object/Concentrate Recipe")]
public class KSD_ConcentrateRecipe:ScriptableObject
{
    public List<KSD_PerfumeMaterialInfo> NeedMaterials;

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

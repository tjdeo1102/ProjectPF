using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Note Recipe Data", menuName = "Scriptable Object/Note Recipe")]
public class KSD_NoteRecipe: ScriptableObject
{
    public List<KSD_PerfumeNoteInfo> NeedConcentrates;

    private void OnValidate()
    {
        if (NeedConcentrates != null &&
            NeedConcentrates.Count != 2)
        {
            Debug.Log($"{nameof(NeedConcentrates)} 개수는 2개 고정");
            int count = NeedConcentrates.Count;

            if (NeedConcentrates.Count > 2) NeedConcentrates.RemoveRange(2, count - 2);
            else
            {
                while (NeedConcentrates.Count != 2)
                {
                    NeedConcentrates.Add(null);
                }
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_FragmentMaterial : MonoBehaviour
{
    // 조각 재료 오브젝트 할당
    [SerializeField] GameObject fragmentMaterial;
    [SerializeField] string fragmentMaterialName;

    private int SliceCount = 0;
    private void Start()
    {
        fragmentMaterialName = fragmentMaterial.name;
    }

    public void OnSlices()
    {
        SliceCount++;

        if (SliceCount >= 4)
        {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_CauldronWater : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] KSD_CauldronController controller;

    private Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (controller == null) return;

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_ProcessPercentage", controller.CurrentPercentage);

        if (controller.IsFail) propertyBlock.SetFloat("_IsFail", 1);
        else propertyBlock.SetFloat("_IsFail", 0);
        
        renderer.SetPropertyBlock(propertyBlock);
    }
}

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

        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_ProcessPercentage", controller.CurrentPercentage);
        propertyBlock.SetColor("_FinishColor", controller.ResultNoteInfo.GetColorByName(controller.ResultNoteInfo.Name));

        if (controller.IsFinish 
            && controller.ResultNoteInfo.Name == PerfumeNoteName.Null
            && controller.ResultNoteInfo.NoteCount < 1) propertyBlock.SetFloat("_IsFail", 1);
        else propertyBlock.SetFloat("_IsFail", 0);


        renderer.SetPropertyBlock(propertyBlock);
    }
}

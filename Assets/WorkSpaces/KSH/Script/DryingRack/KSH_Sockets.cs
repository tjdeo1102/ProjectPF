using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_Sockets : MonoBehaviour
{
    [Header("건조 시간")]
    [SerializeField] private float duration;

    private KSH_DryingRacks dryingRacks;
    private Material materials;
    private Color colors;

    private void Awake()
    {
        colors = new Color(160 / 255f, 105 / 255f, 55 / 255f);
    }

    private void OnTriggerEnter(Collider other)
    {
        dryingRacks = other.GetComponent<KSH_DryingRacks>();
        if (dryingRacks.Iscolor == false)
        {
            materials = other.GetComponent<Renderer>().material;
            other.gameObject.layer = 4;
            FragmentMaterial(materials, other, dryingRacks);
        }
        //if (other.gameObject.CompareTag("Ingredient"))
        //{

        //}
    }

    private void FragmentMaterial(Material material, Collider other, KSH_DryingRacks dryingRacks)
    {
        material.DOColor(colors, duration).OnComplete(() =>
        {
            other.gameObject.layer = 0;
            dryingRacks.Iscolor = true;
        });
    }
}

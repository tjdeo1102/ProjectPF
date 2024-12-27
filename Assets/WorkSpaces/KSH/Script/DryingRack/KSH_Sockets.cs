using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_Sockets : MonoBehaviour
{
    [Header("건조 시간")]
    [SerializeField] private float duration;

    private XRSocketInteractor xrSocket;
    private KSH_DryingRacks dryingRacks;
    private Material materials;
    private Color colors;

    private void Awake()
    {
        xrSocket = GetComponent<XRSocketInteractor>();
        colors = new Color(160 / 255f, 105 / 255f, 55 / 255f);
        duration = KSH_DryingRackManager.Instance.Times;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 대체 API를 사용하여 빈 소켓인지 확인
        if (xrSocket.hasSelection)
        {
            dryingRacks = other.GetComponent<KSH_DryingRacks>();
            if (dryingRacks.Iscolor == false)
            {
                materials = other.GetComponent<Renderer>().material;
                other.gameObject.layer = 4;
                FragmentMaterial(materials, other, dryingRacks);
            }
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

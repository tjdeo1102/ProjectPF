using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_Sockets : MonoBehaviour
{
    [Header("건조 시간")]
    [SerializeField] private float duration;

    private KSH_Tongs tongs;

    // 소켓에 물체가 있는지 확인용
    private XRSocketInteractor xrSocket;
    // 건조된 물체인지 확인용
    private KSH_DryingRacks dryingRacks;
    private Material materials;
    private Color colors;

    private void Awake()
    {
        xrSocket = GetComponent<XRSocketInteractor>();
        colors = new Color(160 / 255f, 105 / 255f, 55 / 255f);
    }

    private void Start()
    {
        // 초기 값 설정
        duration = KSH_DryingRackManager.Instance.Times;

        // 값 변경 이벤트 구독
        KSH_DryingRackManager.Instance.OnTimesChanged += UpdateDuration;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (KSH_DryingRackManager.Instance != null)
        {
            KSH_DryingRackManager.Instance.OnTimesChanged -= UpdateDuration;
        }
    }

    private void UpdateDuration(int newTimes)
    {
        duration = newTimes;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 빈 소켓인지 확인
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

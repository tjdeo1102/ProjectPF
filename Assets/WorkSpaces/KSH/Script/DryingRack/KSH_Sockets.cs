using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class KSH_Sockets : MonoBehaviourPun
{
    [Header("건조 시간")]
    [SerializeField] private int duration;

    [Header("건조 색")]
    [SerializeField] private float targetValue; // 목표 색상의 밝기 값

    private KSH_Tongs tongs;

    // 소켓에 물체가 있는지 확인용
    private XRSocketInteractor xrSocket;
    // 건조된 물체인지 확인용
    private KSH_DryingRacks dryingRacks;


    private void Awake()
    {
        xrSocket = GetComponent<XRSocketInteractor>();
    }

    private void Start()
    {
        // 초기 값 설정
        duration = KSH_DryingRackManager.Instance.Times;
        targetValue = KSH_DryingRackManager.Instance.TargetValue;

        // 값 변경 이벤트 구독
        KSH_DryingRackManager.Instance.OnTimesChanged += UpdateDuration;
        KSH_DryingRackManager.Instance.OnTargetValueChanged += UpdateTargetValue;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (KSH_DryingRackManager.Instance != null)
        {
            KSH_DryingRackManager.Instance.OnTimesChanged -= UpdateDuration;
            KSH_DryingRackManager.Instance.OnTargetValueChanged -= UpdateTargetValue;
        }
    }

    private void UpdateDuration(int newTimes)
    {
        duration = newTimes;
    }

    private void UpdateTargetValue(float newTargetValue)
    {
        targetValue = newTargetValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 빈 소켓인지 확인
        if (xrSocket.hasSelection)
        {
            dryingRacks = other.GetComponent<KSH_DryingRacks>();
            if (dryingRacks == null)
            {
                Debug.Log($"dryingRacks가 없는 오브젝트: {other.gameObject.name}");
                return;
            }
            if (dryingRacks.Iscolor == false)
            {
                other.gameObject.layer = 9;
                int viewID = other.GetComponent<PhotonView>().ViewID;
                photonView.RPC("RPC_FragmentMaterial", RpcTarget.All, viewID);
            }
        }
    }
    [PunRPC]
    private void RPC_FragmentMaterial(int objectViewID) //GameObject other, KSH_DryingRacks dryingRacks)
    {
        GameObject other = PhotonView.Find(objectViewID).gameObject;
        if (other == null)
        {
            Debug.Log("오브젝트를 찾을 수 없습니다.");
            return;
        }

        KSH_DryingRacks dryingRacks = other.GetComponent<KSH_DryingRacks>();
        if (dryingRacks == null)
        {
            Debug.Log($"dryingRacks가 없는 오브젝트: {other.gameObject.name}");
            return;
        }

        // 부모 오브젝트의 모든 자식 순회
        Renderer[] childRenderers = other.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in childRenderers)
        {
            // 자식 오브젝트의 메터리얼 복사본 생성
            Material childMaterial = new Material(renderer.material);
            renderer.material = childMaterial;

            // 현재 색상을 HSV로 변환
            Color.RGBToHSV(childMaterial.color, out float h, out float s, out float v);

            // 목표 밝기 값(V) 설정
            float targetV = targetValue;

            // HSV를 RGB로 변환하여 새로운 색상 생성
            Color updatedColor = Color.HSVToRGB(h, s, targetV);

            childMaterial.DOColor(updatedColor, duration).OnComplete(() =>
        {
            other.gameObject.layer = 0;
            dryingRacks.Iscolor = true;
            dryingRacks.Isdry = true;
        });
        }
    }
}

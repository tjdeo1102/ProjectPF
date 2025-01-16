using DG.Tweening;
using Photon.Pun;
using System.Collections;
using UnityEngine;

// FractureObject 클래스는 특정 오브젝트가 충돌 시 파괴되고 
// 파편 효과를 생성하며, 멀티플레이어 환경에서 이 동작을 동기화하는 역할을 합니다.
public class KSH_FractureObject2 : MonoBehaviour
{
    private PhotonView photonView;

    // 생성할 오브젝트
    [SerializeField] private GameObject powderRawMaterials;
    [SerializeField] private string powderRawMaterialsName;
    private Vector3 position;

    // 메인 오브젝트의 메터리얼
    [SerializeField] private Material mainMaterial;

    // 복사본 오브젝트의 메터리얼
    private Material copiedMaterial;

    // 메인 오브젝트의 MeshRenderer
    [SerializeField] private MeshRenderer[] meshRenderer;

    // 건조 확인
    [SerializeField] private KSH_DryingRacks dryingRacks;

    // 막자 확인
    [SerializeField] private KSH_Plate plate;

    // 자식 오브젝트들의 MeshRenderer 배열
    [SerializeField] private MeshRenderer[] meshRenderers;

    // 자식 오브젝트들의 Rigidbody 배열 (파편 효과에 사용)
    [SerializeField] private Rigidbody[] rigidbodies;

    // 파편 오브젝트 (파괴 후 활성화될 오브젝트)
    [SerializeField] private GameObject[] frags;

    // 파편에 적용할 물리적 힘의 크기
    [SerializeField] private float forcePower;

    // 페이드아웃 딜레이 (0.1초 대기)
    private WaitForSeconds fadeDelay = new WaitForSeconds(0.1f);

    // 방망이 충돌을 추적하는 변수
    [SerializeField] private int collisionThreshold = 2; // 각 단계마다 충돌 횟수
    private int collisionCount = 0; // 현재 충돌 횟수
    private int currentFragIndex = 0; // 현재 활성화된 파편 인덱스

    private bool isfrags = false;
    private float lastSliceTime = 0f; // 마지막 충돌 시간
    [SerializeField] private float sliceCooldown = 0.3f; // 쿨타임 (초)

    [Header("소멸 시간")]
    [SerializeField] private float fadeDuration = 2.0f; // 사라지는 시간
    private bool isFadingOut = false;

    private void Start()
    {
        powderRawMaterialsName = "PerfumeMaterials/Process/" + powderRawMaterials.name;
        position = transform.position;
    }

    private void Awake()
    {
        GameObject plates = GameObject.FindWithTag("Plate");
        if (plates != null)
        {
            plate = plates.GetComponent<KSH_Plate>();
        }
        else
        {
            Debug.LogWarning("태그 'Plate'를 가진 오브젝트가 없습니다!");
        }

        photonView = GetComponent<PhotonView>();

        // 메인 오브젝트의 MeshRenderer를 가져옵니다.
        meshRenderer = GetComponentsInChildren<MeshRenderer>();

        dryingRacks = GetComponent<KSH_DryingRacks>();

        // mainMaterial의 복사본 생성
        copiedMaterial = new Material(mainMaterial);

        Transform targetTransform = transform.Find("Piece");
        if (targetTransform != null)
        {
            meshRenderers = targetTransform.GetComponentsInChildren<MeshRenderer>(true);
        }

        // 모든 자식 오브젝트의 Rigidbody 배열을 가져옵니다.
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "TestBat"인지 확인
        if (!other.gameObject.CompareTag("TestBat")) return;

        // 쿨타임 검사: 쿨타임이 지나지 않으면 충돌을 실행하지 않음
        if (Time.time - lastSliceTime < sliceCooldown) return;

        if (!dryingRacks.Isdry) return;

        if (!plate.IsFlower) return;

        other.gameObject.layer = 9;
        collisionCount++; // 충돌 횟수 증가
        lastSliceTime = Time.time;

        if (!isfrags)
        {
            photonView.RPC(nameof(RPC_SetGameObject), RpcTarget.All);
            isfrags = true;
        }

        photonView.RPC(nameof(RPC_AuidoPlay), RpcTarget.All);
        photonView.RPC(nameof(RPC_EffectPlay), RpcTarget.All);

        // 충돌 횟수가 각 단계(2, 4, 6)에 도달할 때마다 새로운 파편 활성화
        if (collisionCount >= (currentFragIndex + 1) * collisionThreshold && currentFragIndex < frags.Length)
        {
            photonView.RPC(nameof(RPC_ActivateFragment), RpcTarget.All, currentFragIndex);

            // 모든 파괴 오브젝트가 활성화되었는지 확인하고 페이드아웃 시작
            if (currentFragIndex == frags.Length && !isFadingOut)
            {
                photonView.RPC(nameof(StartFadeOut), RpcTarget.All, transform.position);
            }
        }
    }

    // 모든 파괴 오브젝트를 서서히 사라지게 함
    [PunRPC]
    private void StartFadeOut(Vector3 pos)
    {
        isFadingOut = true;
        copiedMaterial.DOFade(0, fadeDuration).OnComplete(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Instantiate(powderRawMaterialsName, pos, Quaternion.identity);
            }

            // 일정 시간 후 오브젝트 삭제
            DOVirtual.DelayedCall(1f, () => {
                if (photonView.IsMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
                else if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            });
        });
    }

    // 특정 파편 오브젝트를 활성화하고 자식 Rigidbody들의 isKinematic을 false로 설정
    [PunRPC]
    private void RPC_ActivateFragment(int index)
    {
        if (index >= frags.Length) return;

        // 파편 오브젝트의 모든 자식 Rigidbody 활성화
        Rigidbody[] fragRigidbodies = frags[index].GetComponentsInChildren<Rigidbody>();
        foreach (var rb in fragRigidbodies)
        {
            rb.isKinematic = false; // 물리 효과 적용 가능
        }
        currentFragIndex++; // 다음 파편 활성화를 위해 인덱스 증가
    }

    [PunRPC]
    private void RPC_SetGameObject()
    {
        // 모든 메인 MeshRenderer를 비활성화
        foreach (var renderer in meshRenderer)
        {
            renderer.enabled = false;
        }

        // 모든 파편 오브젝트 활성화
        foreach (var frag in frags)
        {
            frag.SetActive(true);
        }

        // 자식 MeshRenderer의 모든 Material을 부모 Material로 설정
        foreach (var childRenderer in meshRenderers)
        {
            Material[] childMaterials = childRenderer.materials;
            for (int i = 0; i < childMaterials.Length; i++)
            {
                // 공유된 mainMaterial 사용
                childMaterials[i] = copiedMaterial;
            }
            childRenderer.materials = childMaterials;
        }
    }

    [PunRPC]
    private void RPC_AuidoPlay()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Mortar_play);
    }

    [PunRPC]
    private void RPC_EffectPlay()
    {
        KSH_EffectManager.Instance.PlayEffect(KSH_EffectManager.Effect.Mortar_play, transform.position);
    }

    //    // 페이드아웃 효과를 처리하는 코루틴
    //    IEnumerator FadeOut()
    //    {
    //        float alpha = 1; // 시작 알파 값
    //        Color color = meshRenderers[0].material.color; // 첫 번째 Material의 초기 색상

    //        // 알파 값이 0이 될 때까지 반복
    //        while (alpha > 0)
    //        {
    //            color.a = alpha; // 알파 값을 업데이트

    //            // 모든 자식 MeshRenderer의 Material 색상을 업데이트
    //            foreach (MeshRenderer renderer in meshRenderers)
    //            {
    //                foreach (Material mat in renderer.materials)
    //                {
    //                    mat.color = color;
    //                }
    //            }

    //            alpha -= 0.04f; // 알파 값을 감소
    //            yield return fadeDelay; // 대기
    //        }
    //    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using DG.Tweening;
using static Photon.Voice.OpusCodec;

public class KSH_FragmentMaterial : MonoBehaviour
{
    // 조각 재료 오브젝트 할당
    [SerializeField] GameObject fragmentMaterial;
    [SerializeField] string fragmentMaterialName;
    private Vector3 position;
    private PhotonView photonView;

    // 오브젝트 메터리얼 복사본 생성
    private MeshRenderer meshRenderer;
    [SerializeField] private Material material;

    // 자식 오브젝트들의 MeshRenderer 배열
    [SerializeField] private MeshRenderer[] meshRenderers;

    [Header("소멸 시간")]
    [SerializeField] private float fadeDuration = 2.0f; // 사라지는 시간

    // 슬라이스 횟수
    private int SliceCount = 0;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // 메인 오브젝트의 MeshRenderer를 가져옵니다.
        meshRenderer = GetComponent<MeshRenderer>();

        material = meshRenderer.material;
    }

    private void Start()
    {
        fragmentMaterialName = fragmentMaterial.name;
        position = transform.position;
    }

    public void OnPositon()
    {
        position = transform.position;
    }

    [PunRPC]
    public void OnAudio()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Cut);
    }

    public void OnSlices()
    {
        SliceCount++;
        if (PhotonNetwork.IsMasterClient) // MasterClient만 RPC 호출
        {
            photonView.RPC(nameof(OnAudio), RpcTarget.All);
        }
        // 모든 자식 오브젝트의 MeshRenderer 배열을 가져옵니다.
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        // 자식 MeshRenderer의 모든 Material을 부모 Material로 설정
        foreach (var childRenderer in meshRenderers)
        {
            Material[] childMaterials = childRenderer.materials; // 자식의 Material 배열
            for (int i = 0; i < childMaterials.Length; i++)
            {
                childMaterials[i] = material; // 모든 Material에 부모 Material 복사본 할당
            }
            childRenderer.materials = childMaterials; // 업데이트된 배열을 다시 설정
        }
        if (SliceCount >= 3)
        {
            if (PhotonNetwork.IsMasterClient) // MasterClient만 RPC 호출
            {
                photonView.RPC(nameof(FragmentMaterial), RpcTarget.All, position);
            }
        }
    }

    [PunRPC]
    private void FragmentMaterial(Vector3 pos)
    {
        material.DOFade(0, fadeDuration).OnComplete(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // KSH_EffectManager.Instance.PlayEffect(KSH_EffectManager.Effect.Fire, transform.position);
                PhotonNetwork.Instantiate(fragmentMaterialName, pos, Quaternion.identity);
            }
            DOVirtual.DelayedCall(0.1f, () =>
            {
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
}

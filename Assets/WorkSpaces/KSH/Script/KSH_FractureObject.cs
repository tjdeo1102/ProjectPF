using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// FractureObject 클래스는 특정 오브젝트가 충돌 시 파괴되고 
// 파편 효과를 생성하며, 멀티플레이어 환경에서 이 동작을 동기화하는 역할을 합니다.
public class KSH_FractureObject : MonoBehaviour
{
    // 메인 오브젝트의 MeshRenderer
    MeshRenderer meshRenderer;

    // 자식 오브젝트들의 MeshRenderer 배열
    MeshRenderer[] meshRenderers;

    // 자식 오브젝트들의 Rigidbody 배열 (파편 효과에 사용)
    [SerializeField]Rigidbody[] rigidbodies;

    // 파편 오브젝트 (파괴 후 활성화될 오브젝트)
    [SerializeField] GameObject frags;

    // 파편에 적용할 물리적 힘의 크기
    [SerializeField] float forcePower;

    // 페이드아웃 딜레이 (0.1초 대기)
    WaitForSeconds fadeDelay = new WaitForSeconds(0.1f);

    // Awake: 컴포넌트와 자식 오브젝트 초기화
    private void Awake()
    {
        // 메인 오브젝트의 MeshRenderer를 가져옵니다.
        meshRenderer = GetComponent<MeshRenderer>();

        // 모든 자식 오브젝트의 MeshRenderer 배열을 가져옵니다.
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);

        // 모든 자식 오브젝트의 Rigidbody 배열을 가져옵니다.
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

    public void StartFadeOutRPC()
    {
        // 메인 MeshRenderer를 비활성화
        meshRenderer.enabled = false;

        // 파편 오브젝트 활성화
        frags.SetActive(true);

        // 각 Rigidbody에 임의의 방향으로 물리적 힘 추가
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 ranPos = Random.insideUnitSphere; // 임의의 방향 벡터
            ranPos.y += 0.3f; // 약간 위쪽으로 추가 힘
            rigidbodies[i].AddForce(ranPos * forcePower, ForceMode.Impulse);
        }

        // 페이드아웃 시작
        StartCoroutine(FadeOut());
    }

    // 페이드아웃 효과를 처리하는 코루틴
    IEnumerator FadeOut()
    {
        float alpha = 1; // 시작 알파 값
        Color color = meshRenderers[0].material.color; // 첫 번째 Material의 초기 색상

        // 알파 값이 0이 될 때까지 반복
        while (alpha > 0)
        {
            color.a = alpha; // 알파 값을 업데이트

            // 모든 자식 MeshRenderer의 Material 색상을 업데이트
            foreach (MeshRenderer renderer in meshRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = color;
                }
            }

            alpha -= 0.04f; // 알파 값을 감소
            yield return fadeDelay; // 대기
        }
    }
}
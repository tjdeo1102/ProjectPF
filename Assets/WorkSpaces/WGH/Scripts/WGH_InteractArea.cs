using System.Collections;
using UnityEngine;

// 상호작용 콜라이더 탐지용 클래스
public class WGH_InteractArea : MonoBehaviour
{
    [SerializeField] WGH_NPCController controller;
    [SerializeField, Tooltip("실패기준 횟수")] int maxCount;
    private int curCount;

    private void Awake()
    {
        controller = GetComponentInParent<WGH_NPCController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_PerfumeRecipe perfume) && curCount < maxCount)
        {
            if (perfume.PerfumeType == controller.PerfumeType)
            {
                // 성공하면 성공 감정표현 후 퇴장
                StartCoroutine(PurchaseRoutine());
            }
            else
            {
                // 실패하면 절망 감정표현 후 실패횟수 1회 추가
                controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
                curCount++;
                if (curCount >= maxCount)
                {
                    // 실패횟수가 설정된 수에 도달하면 퇴장
                    StartCoroutine(FailRoutine());
                }
            }
        }
    }

    IEnumerator PurchaseRoutine()
    {
        controller.SelectReactUINetwork((int)E_ReactUiType.BEST);
        yield return new WaitForSeconds(2);
        controller.ChangeStateNetwork((int)E_StateType.EXIT);
    }

    IEnumerator FailRoutine()
    {
        yield return new WaitForSeconds(2);
        controller.ChangeStateNetwork((int)E_StateType.EXIT);
    }
}

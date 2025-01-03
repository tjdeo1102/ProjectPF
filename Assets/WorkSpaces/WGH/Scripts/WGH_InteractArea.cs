using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 상호작용 콜라이더 탐지용 클래스
public class WGH_InteractArea : MonoBehaviour
{
    [SerializeField] WGH_NPCController controller;
    [SerializeField, Tooltip("실패기준 횟수")] int maxCount;
    private int curCount;
    [SerializeField] private bool isCheck;

    public event Action OnChangedSmellStick;

    private void Awake()
    {
        controller = GetComponentInParent<WGH_NPCController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_SmellStick smellStick))
        {
            controller.SmellStick = smellStick;
            OnChangedSmellStick?.Invoke();
        }
        if (other.gameObject.TryGetComponent(out LSY_PotionReceiver potion) && curCount < maxCount)
        {
            if (isCheck == false && potion.perfumeName == controller.PerfumeType
                 && potion.e_BottleType == controller.BottleType) //TODO : 시연님 스크립트 머지 후 포션 결과물의 병타입도 비교해야 함
            {
                // 성공하면 성공 감정표현 후 퇴장
                StartCoroutine(PurchaseRoutine());
            }
            else if (isCheck == false && (potion.perfumeName != controller.PerfumeType
                 || potion.e_BottleType != controller.BottleType))
            {
                // 실패하면 절망 감정표현 후 실패횟수 1회 추가
                // 실패횟수 2회 이상일 시 퇴장
                StartCoroutine(CheckTimeDelayRoutine());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_SmellStick smellStick))
        {
            controller.SmellStick = null;
        }
    }

    IEnumerator PurchaseRoutine()
    {
        isCheck = true;
        controller.SelectReactUINetwork((int)E_ReactUiType.BEST);
        KSD_GameManager.Instance.AddFinishPlayerCount(1);
        yield return new WaitForSeconds(2);
        controller.ChangeStateNetwork((int)E_StateType.EXIT);
    }

    IEnumerator CheckTimeDelayRoutine()
    {
        isCheck = true;
        curCount++;
        controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
        yield return new WaitForSeconds(2);
        if (curCount >= maxCount)
        {
            // 실패횟수가 설정된 수에 도달하면 퇴장
            controller.ChangeStateNetwork((int)E_StateType.EXIT);
        }
        else
        {
            isCheck = false;
        }
    }
}

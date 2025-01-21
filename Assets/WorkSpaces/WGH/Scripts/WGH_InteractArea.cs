using Photon.Pun;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 상호작용 콜라이더 탐지용 클래스
public class WGH_InteractArea : MonoBehaviour
{
    [SerializeField] WGH_NPCController controller;
    
    public event Action OnChangedSmellStick;

    [SerializeField, Tooltip("실패기준 횟수")] int maxCount;
    private int curCount;
    [SerializeField] private bool isCheck;

    private void Awake()
    {
        controller = GetComponentInParent<WGH_NPCController>();
        maxCount = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_SmellStick smellStick))
        {
            controller.SmellStick = smellStick;
            OnChangedSmellStick?.Invoke();
        }
        if (other.gameObject.TryGetComponent(out LSY_PotionReceiver potion) && curCount < maxCount && potion.perfumeName != E_WGH_PerfumeType.NONE)
        {
            if (isCheck == false && potion.perfumeName == controller.PerfumeType
                 && potion.e_BottleType == controller.BottleType)
            {
                // 성공하면 성공 감정표현 후 퇴장
                StartCoroutine(PurchaseRoutine(potion));
            }
            else if (isCheck == false && (potion.perfumeName != controller.PerfumeType
                 || potion.e_BottleType != controller.BottleType))
            {
                // 실패하면 절망 감정표현 후 실패횟수 1회 추가
                // 실패횟수 2회 이상일 시 퇴장
                StartCoroutine(CheckTimeDelayRoutine(potion));
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

    IEnumerator PurchaseRoutine(LSY_PotionReceiver potion)
    {
        isCheck = true;
        controller.SelectReactUINetwork((int)E_ReactUiType.SUCCESS);
        controller.SetAnimNetwork("Perfume");
        controller.SetEmotion(5);
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Guest_success);

        KSD_GameManager.Instance.AddFinishPlayerCount(1);
        PhotonView potionView = potion.GetComponent<PhotonView>();

        yield return new WaitForSeconds(2);

        potionView.RPC("DestroyPotion", RpcTarget.All);
        yield return null;
        if (curCount == 0 && KSD_GameManager.Instance != null)
        {
            KSD_GameManager.Instance.CurrentStageInfo.StageMoney += 100;
        }
        else if (curCount == 1 && KSD_GameManager.Instance != null)
        {
            KSD_GameManager.Instance.CurrentStageInfo.StageMoney += 80;
        }
        Debug.Log(KSD_GameManager.Instance.CurrentStageInfo.StageMoney);
        controller.SetEmotion(0);
        curCount = 0;
        controller.ChangeStateNetwork((int)E_StateType.EXIT);
        WGH_NPCCreator.Instance.isCounter = false;
        
        yield return null;
        isCheck = false;
    }

    IEnumerator CheckTimeDelayRoutine(LSY_PotionReceiver potion)
    {
        isCheck = true;
        curCount++;
        controller.SetAnimNetwork("No");
        controller.SelectReactUINetwork((int)E_ReactUiType.FAIL);
        controller.SetEmotion(6);
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Guest_fall);

        PhotonView potionView = potion.GetComponent<PhotonView>();
        yield return new WaitForSeconds(2);
        controller.SetEmotion(0);

        potionView.RPC("DestroyPotion", RpcTarget.All);
        yield return null;

        if (curCount >= maxCount)
        {
            // 실패횟수가 설정된 수에 도달하면 퇴장
            WGH_NPCCreator.Instance.isCounter = false;
            curCount = 0;
            controller.ChangeStateNetwork((int)E_StateType.EXIT);
            yield return null;
            isCheck = false;
        }
        else
        {
            isCheck = false;
        }
    }
}

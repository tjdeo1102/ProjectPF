using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public enum E_ReactUiType
{
    BEST,
    LIKE,
    QUESTION,
    DESPAIR
}


public class WGH_NPCWait : INPCState
{
    private WGH_NPCController controller;
    private int randomNum;
    [SerializeField] private WGH_SmellStick smellStick;
    public WGH_NPCWait(WGH_NPCController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        randomNum = Random.Range(0, (int)E_BottleType.E_BottleType_MAX);
        if (PhotonNetwork.IsMasterClient)
        {
            controller.SelectOrderUINetwork(randomNum, (int)controller.PerfumeType - 1);
        }
        Debug.Log("Wait 상태");
        // 상호작용 콜라이더에 시향지가 들어와서 시향지 변수에 배정될때 그 시향지의 스크립트의 이벤트에 함수를 등록하기
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick += FindSmellStick;
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// 리액션 이벤트 등록 함수
    /// </summary>
    private void FindSmellStick()
    {
        controller.SmellStick.OnBestInteract += BestEmotion;
        controller.SmellStick.OnLikeInteract += LikeEmotion;
        controller.SmellStick.OnQuestionInteract += QuestionEmotion;
        controller.SmellStick.OnDespairInteract += DespairEmotion;
    }

    private void BestEmotion()
    {
        controller.SelectReactUINetwork((int)E_ReactUiType.BEST);
    }

    private void LikeEmotion()
    {
        controller.SelectReactUINetwork((int)E_ReactUiType.LIKE);
    }

    private void QuestionEmotion()
    {
        controller.SelectReactUINetwork((int)E_ReactUiType.QUESTION);
    }

    private void DespairEmotion() 
    {
        controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
    }

    public void Exit() 
    {
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick -= FindSmellStick;
    }
}

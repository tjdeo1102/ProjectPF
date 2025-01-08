using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WGH_NPCSmellTest : INPCState
{
    private WGH_NPCController controller;

    private NavMeshAgent agent;

    public WGH_NPCSmellTest(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        // npc 선호 이모지 내리기
        controller.PerfumeUI.gameObject.SetActive(false);
        controller.BottleUI.gameObject.SetActive(false);
        // npc 탐지 영역 true
        controller.InteractionArea.gameObject.SetActive(true);
        
        controller.SmellTestEndButton.interactable = true;
        controller.SmellTestEndButton.onClick.AddListener(ChangeWaitState);

        // 상호작용 콜라이더에 시향지가 들어와서 시향지 변수에 배정될때 그 시향지의 스크립트의 이벤트에 함수를 등록하기
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick += FindSmellStick;
    }

    public void ChangeWaitState()
    {
        controller.ChangeStateNetwork((int)E_StateType.WAIT);
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
        controller.PerfumeUI.gameObject.SetActive(true);
        controller.BottleUI.gameObject.SetActive(true);

        controller.InteractionArea.gameObject.SetActive(false);

        controller.SmellTestEndButton.onClick.RemoveListener(ChangeWaitState);
        controller.SmellTestEndButton.interactable = false;

        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick -= FindSmellStick;
    }
}

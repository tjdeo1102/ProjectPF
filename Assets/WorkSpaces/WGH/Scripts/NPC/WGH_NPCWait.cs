using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using UnityEngine.UI;

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
    private NavMeshAgent agent;
    public WGH_NPCWait(WGH_NPCController controller, NavMeshAgent agent)
    {
        this.controller = controller;
        this.agent = agent;
    }

    public void Enter()
    {
        // npc 탐지 영역 true
        controller.InteractionArea.gameObject.SetActive(true);
        // 시향 노트 찾기
        controller.TestNote = GameObject.FindGameObjectWithTag("TestNote");
        WGH_InteractionNote[] testNotes = controller.TestNote.GetComponentsInChildren<WGH_InteractionNote>();
        // 시향 노트 손님에 현재 손님 할당
        foreach(WGH_InteractionNote note in testNotes)
        {
            note.Customer = controller;
        }

        randomNum = Random.Range(0, (int)E_BottleType.E_BottleType_MAX);
        if (PhotonNetwork.IsMasterClient)
        {
            controller.SelectOrderUINetwork(randomNum, (int)controller.PerfumeType - 1);
        }

        // 상호작용 콜라이더에 시향지가 들어와서 시향지 변수에 배정될때 그 시향지의 스크립트의 이벤트에 함수를 등록하기
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick += FindSmellStick;
    }

    public void OnUpdate()
    {
        
    }
    

    public void Exit() 
    {
        // npc 탐지 영역 false
        controller.InteractionArea.gameObject.SetActive(false);
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick -= FindSmellStick;
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
        Debug.Log("실망");
        controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
    }
}

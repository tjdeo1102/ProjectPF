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
        // 시향 노트 찾기
        controller.TestNote = GameObject.FindGameObjectWithTag("TestNote");
        WGH_InteractionNote[] testNotes = controller.TestNote.GetComponentsInChildren<WGH_InteractionNote>();
        // 시향 노트 손님에 현재 손님 할당
        foreach(WGH_InteractionNote note in testNotes)
        {
            note.Customer = controller;
        }
        // 시향 버튼 & 시향 종료버튼 찾기
        controller.SmellTestStartButton = controller.TestNote.transform.GetChild(5).GetChild(5).gameObject.GetComponent<Button>();
        controller.SmellTestEndButton = controller.TestNote.transform.GetChild(5).GetChild(6).gameObject.GetComponent<Button>();

        controller.SmellTestStartButton.interactable = true;
        controller.SmellTestStartButton.onClick.AddListener(ChangeSmellState);

        randomNum = Random.Range(0, (int)E_BottleType.E_BottleType_MAX);
        if (PhotonNetwork.IsMasterClient)
        {
            controller.SelectOrderUINetwork(randomNum, (int)controller.PerfumeType - 1);
        }
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// 시향 상태로 전환 하는 메서드
    /// </summary>
    public void ChangeSmellState()
    {
        controller.SmellTestStartButton.interactable = false;
        controller.ChangeStateNetwork((int)E_StateType.SMELL);
    }
    

    public void Exit() 
    {
        controller.SmellTestStartButton.onClick.RemoveListener(ChangeSmellState);
    }
}

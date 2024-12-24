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

public enum E_BottleType // 플로팅 되는 병 이미지
{
    a,
    b,
    c,
    d,
    e,
    E_BottleType_MAX
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
            controller.SelectBottleUINetwork(randomNum);
        }
        Debug.Log("Wait 상태");
        controller.InteractionArea.GetComponent<WGH_InteractArea>().OnChangedSmellStick += FindSmellStick;
    }

    public void OnUpdate()
    {
        
    }

    private void FindSmellStick()
    {
        // 시향지가 배정될때 되도록 이벤트
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

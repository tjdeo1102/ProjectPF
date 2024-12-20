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
    e
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
        smellStick = GameObject.FindGameObjectWithTag("SmellStick").GetComponent<WGH_SmellStick>();
        randomNum = Random.Range(0, 5);
        if (PhotonNetwork.IsMasterClient)
        {
            controller.SelectBottleUINetwork(randomNum);
        }
        Debug.Log("Wait 상태");

        smellStick.OnBestInteract += BestEmotion;
        smellStick.OnLikeInteract += LikeEmotion;
        smellStick.OnQuestionInteract += QuestionEmotion;
        smellStick.OnDespairInteract += DespairEmotion;
    }

    public void OnUpdate()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    controller.SelectReactUINetwork((int)E_ReactUiType.BEST);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    controller.SelectReactUINetwork((int)E_ReactUiType.LIKE);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    controller.SelectReactUINetwork((int)E_ReactUiType.QUESTION);
        //}
        //if(Input.GetKeyDown(KeyCode.Alpha4)) 
        //{
        //    controller.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
        //}
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
        
    }
}

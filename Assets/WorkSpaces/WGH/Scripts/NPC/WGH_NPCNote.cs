using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class WGH_NPCNote : MonoBehaviourPun
{
    WGH_NPCController controller;
    private E_WGH_PerfumeType PerfumeType;
    private E_WGH_NoteType BestMaterial;
    private E_WGH_NoteType LikeMaterial;
    private E_WGH_NoteType LikeMaterial2;
    private E_WGH_NoteType QuestionMaterial;
    private E_WGH_NoteType QuestionMaterial2;

    private void Awake()
    {
        controller = GetComponent<WGH_NPCController>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        int randNum = Random.Range(1, (int)E_WGH_PerfumeType.E_PERFUMETYPE_MAX);
        PerfumeType = (E_WGH_PerfumeType)randNum;
        SetPerfumeTypeRPC((int)PerfumeType);
    }

    private void SetPerfumeTypeRPC(int randNum)
    {
        photonView.RPC("SetPerfumeType", RpcTarget.All, randNum);
    }

    [PunRPC]
    private void SetPerfumeType(int randNum)
    {
        switch (randNum)
        {
            case 1:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.FLORAL;
                controller.LikeMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial2 = E_WGH_NoteType.NONE;
                controller.QuestionMaterial = E_WGH_NoteType.CITRUS;
                controller.QuestionMaterial2 = E_WGH_NoteType.GREEN;
                break;
            case 2:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.WOODY;
                controller.LikeMaterial = E_WGH_NoteType.GREEN;
                controller.LikeMaterial2 = E_WGH_NoteType.NONE;
                controller.QuestionMaterial = E_WGH_NoteType.NONE;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 3:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.FRUITY;
                controller.LikeMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial2 = E_WGH_NoteType.NONE;
                controller.QuestionMaterial = E_WGH_NoteType.CITRUS;
                controller.QuestionMaterial2 = E_WGH_NoteType.GREEN;
                break;
            case 4:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.GREEN;
                controller.LikeMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial2 = E_WGH_NoteType.NONE;
                controller.QuestionMaterial = E_WGH_NoteType.WOODY;
                controller.QuestionMaterial2 = E_WGH_NoteType.FLORAL;
                break;
            case 5:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.CITRUS;
                controller.LikeMaterial = E_WGH_NoteType.GREEN;
                controller.LikeMaterial2 = E_WGH_NoteType.NONE;
                controller.QuestionMaterial = E_WGH_NoteType.NONE;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 6:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial = E_WGH_NoteType.WOODY;
                controller.LikeMaterial2 = E_WGH_NoteType.FLORAL;
                controller.QuestionMaterial = E_WGH_NoteType.CITRUS;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 7:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial = E_WGH_NoteType.FRUITY;
                controller.LikeMaterial2 = E_WGH_NoteType.FLORAL;
                controller.QuestionMaterial = E_WGH_NoteType.GREEN;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 8:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial = E_WGH_NoteType.FRUITY;
                controller.LikeMaterial2 = E_WGH_NoteType.CITRUS;
                controller.QuestionMaterial = E_WGH_NoteType.FLORAL;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
            case 9:
                controller.PerfumeType = (E_WGH_PerfumeType)randNum;
                controller.BestMaterial = E_WGH_NoteType.NONE;
                controller.LikeMaterial = E_WGH_NoteType.WOODY;
                controller.LikeMaterial2 = E_WGH_NoteType.GREEN;
                controller.QuestionMaterial = E_WGH_NoteType.FRUITY;
                controller.QuestionMaterial2 = E_WGH_NoteType.NONE;
                break;
        }
    }
}

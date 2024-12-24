using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public class KSD_PerfumeMaterial : MonoBehaviourPun
{
    [Header("재료 상태 설정")]
    [SerializeField] private KSD_PerfumeMaterialInfo perfumeMaterial;
    public KSD_PerfumeMaterialInfo PerfumeMaterial { get; private set; }

    [Header("바꿀 오브젝트 설정")]
    [SerializeField] private GameObject[] changeMaterialObjects;        //재료의 바뀐 모양을 담을 리스트

    [PunRPC]
    private void ChangeToTypeRPC(int objectIdx, byte type)
    {
        ChangeObjectActive(objectIdx);
        PerfumeMaterial.Type = (PerfumeMaterialType)type;
    }

    /// <summary>
    /// 재료의 타입을 바꿔주는 함수 (Ex. 과실이 큰 과일 => 과실이 작은 과일)
    /// </summary>
    public void ChangeToType(int objectIdx, PerfumeMaterialType type)

    {
        // 모든 클라이언트에게 해당 재료 타입이 바뀌는 것을 전달
        photonView.RPC("ChangeToTypeRPC", RpcTarget.All, objectIdx, (byte)type);
    }

    private void ChangeObjectActive(int objectIdx)
    {
        for (int i = 0; i < changeMaterialObjects.Length; i++)
        {
            if (changeMaterialObjects[i] == null) continue;
            if (objectIdx == i)
            {
                changeMaterialObjects[i].SetActive(true);
            }
            else
            {
                changeMaterialObjects[i].SetActive(false);
            }
        }
    }
}

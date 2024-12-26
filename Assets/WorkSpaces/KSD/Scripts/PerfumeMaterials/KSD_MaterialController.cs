using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public class KSD_MaterialController : MonoBehaviourPun
{
    [Header("재료 상태 설정")]
    [SerializeField] private KSD_PerfumeMaterialInfo perfumeMaterial;
    public KSD_PerfumeMaterialInfo PerfumeMaterial { get; private set; }

    [Header("가공 전/후 오브젝트 설정")]
    [SerializeField] private GameObject rawObject;
    [SerializeField] private GameObject processObject;        // 재료 가공 후, 오브젝트

    [PunRPC]
    private void ChangeToMaterialRPC(int objectIdx, byte type, byte state)
    {
        if (rawObject != null) rawObject.SetActive(false);
        if (processObject != null) processObject.SetActive(true);
        PerfumeMaterial.Type = (PerfumeMaterialType)type;
        PerfumeMaterial.State = (PerfumeMaterialState)state;
    }

    /// <summary>
    /// 재료의 타입을 바꿔주는 함수 (Ex. 과실이 큰 과일 => 과실이 작은 과일)
    /// </summary>
    public void ChangeToType(int objectIdx, PerfumeMaterialType type, PerfumeMaterialState state)

    {
        // 모든 클라이언트에게 해당 재료 타입이 바뀌는 것을 전달
        photonView.RPC("ChangeToTypeRPC", RpcTarget.All, objectIdx, (byte)type, (byte)state);
    }


}

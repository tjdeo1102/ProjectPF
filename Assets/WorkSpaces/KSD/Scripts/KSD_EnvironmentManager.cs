using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KSD_EnvironmentManager : MonoBehaviourPun
{
    [Header("기본 설정")]
    public float PlayTime;
    [SerializeField] private Transform directLight;
    [SerializeField] private float minLightRotation;
    [SerializeField] private float lightRotationLengh;

    private void Start()
    {
        // 방장인 경우, 모든 클라이언트의 PlayTime을 동기화하도록 RPC호출
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("InitTimerRPC", RpcTarget.All);
        }
    }


    /// <summary>
    /// PlayTime을 초기화 시켜주는 함수.
    /// 해당 함수 호출 시, 서버 시간과 동기화 하는 작업 포함
    /// </summary>
    [PunRPC]
    public void InitTimerRPC(PhotonMessageInfo info)
    {
        // Delay 시간만큼 지연 보상 적용
        var delay = Math.Abs(PhotonNetwork.Time - info.SentServerTime);
        PlayTime = (float)delay;
    }

    private void FixedUpdate()
    {
        PlayTime += Time.fixedDeltaTime;
    }

    public void ChangeLight(int currentCount, int maxCount)
    {
        if (directLight == null) return;

        // 클리어한 손님 수에 따라, 점점 밤으로 바뀌도록 구성
        var lightRot = minLightRotation + ((float)currentCount / (float)maxCount) * lightRotationLengh;

        directLight.eulerAngles = new Vector3(lightRot, directLight.rotation.y, directLight.rotation.z);
    }

}

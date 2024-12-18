using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KSD_EnvironmentManager : MonoBehaviourPun
{
    [Header("�⺻ ����")]
    public float PlayTime;
    [SerializeField] private Transform directLight;
    [SerializeField] private float minLightRotation;
    [SerializeField] private float lightRotationLengh;

    private void Start()
    {
        // ������ ���, ��� Ŭ���̾�Ʈ�� PlayTime�� ����ȭ�ϵ��� RPCȣ��
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("InitTimerRPC", RpcTarget.All);
        }
    }


    /// <summary>
    /// PlayTime�� �ʱ�ȭ �����ִ� �Լ�.
    /// �ش� �Լ� ȣ�� ��, ���� �ð��� ����ȭ �ϴ� �۾� ����
    /// </summary>
    [PunRPC]
    public void InitTimerRPC(PhotonMessageInfo info)
    {
        // Delay �ð���ŭ ���� ���� ����
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

        // Ŭ������ �մ� ���� ����, ���� ������ �ٲ�� ����
        var lightRot = minLightRotation + ((float)currentCount / (float)maxCount) * lightRotationLengh;

        directLight.eulerAngles = new Vector3(lightRot, directLight.rotation.y, directLight.rotation.z);
    }

}

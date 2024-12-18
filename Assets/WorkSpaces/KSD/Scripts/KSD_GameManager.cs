using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class KSD_GameManager : MonoBehaviourPun
{
    [Header("�⺻ ����")]
    [SerializeField] private int maxCustomerCount;
    [SerializeField] private int currentStageID;
    [SerializeField] private int returnSceneIndex;

    [Header("���� �Ŵ��� ���� ���")]
    [SerializeField] KSD_EnvironmentManager environmentManager;

    [Header("���� �������� ����")]
    public KSD_StageInfo CurrentStageInfo;

    void Start()
    {
        // �� Ŭ���̾�Ʈ�� �������� ����ȭ�ǵ��� RPCȣ��
        InitStage();
    }

    /// <summary>
    /// Ŭ���̾�Ʈ�� �� ������ �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    public void InitStage()
    {
        string MapOwnerName = null;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MapOwnerName", out object mapOwnerIDObj))
        {
            MapOwnerName = (string)mapOwnerIDObj;
        }
        else
        {
            Debug.Log("MapOwnerName�� �������� �ʾҽ��ϴ�.");
        }

        // ��ȿ� ������ �ִ� ���, ������ ���� ID������ ����, �������� ������ �ҷ���
        if (MapOwnerName.IsNullOrEmpty() == false)
        {
            KSD_SaveLoad.Instance.LoadToDatabase(MapOwnerName, currentStageID).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"���� �ε��ϴ� �� ������ �߻��߽��ϴ�. �κ�� �����մϴ�. \n Error: {task.Exception}");
                    // TODO: �κ�� �����ϴ� �ڵ� �ʿ�
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
                else
                {
                    if (task.Result == null)
                    {
                        Debug.LogWarning("�� �����Ͱ� �����Ƿ� ���ο� �� �����͸� �����մϴ�.");
                        CurrentStageInfo = new KSD_StageInfo();
                        CurrentStageInfo.StageID = currentStageID;
                    }
                    else
                    {
                        Debug.Log("���������� �� �ε� ����");
                        CurrentStageInfo = task.Result;
                    }
                    UpdateEnvironment();
                }
            });
        }
        else
        {
            Debug.LogError("���������� mapOwnerID. �κ�� �����մϴ�.");
            // TODO: �κ�� �����ϴ� �ڵ� �ʿ�
            PhotonNetwork.LoadLevel(returnSceneIndex);
        }
    }

    [PunRPC]
    private void ChangeFinishPlayerCountRPC(int changeCount)
    {

        CurrentStageInfo.FinishPlayerCount += changeCount;

        if (CurrentStageInfo.FinishPlayerCount >= maxCustomerCount)
        {
            // �������� ���
            CurrentStageInfo.StageLevel++;
            CurrentStageInfo.FinishPlayerCount = 0;
        }
        UpdateEnvironment();
    }

    /// <summary>
    /// ���� �Ϸ�� �÷��̾��� ī��Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="changeCount"> �߰��ǰų� ���ҵ� ī��Ʈ </param>
    public void ChangeFinishPlayerCount(int changeCount)
    {
        photonView.RPC("ChangeFinishPlayerCountRPC", RpcTarget.All, changeCount);
    }


    public void SaveAndQuitGame()
    {
        KSD_SaveLoad.Instance.SaveToDatabase(PhotonNetwork.LocalPlayer.NickName, CurrentStageInfo).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("���� �����ϴ� �� ������ �߻��߽��ϴ�. �κ�� �����մϴ�.");
                PhotonNetwork.LoadLevel(returnSceneIndex);
            }
            else
            {
                if (task.Result == true)
                {
                    Debug.Log("���������� �� ���� ����, �κ�� �����մϴ�.");
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
                else
                {
                    Debug.LogError("���� �����ϴ� �� ������ �߻��߽��ϴ�. �κ�� �����մϴ�.");
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
            }
        });
    }

    public void UpdateEnvironment()
    {
        if (environmentManager != null)
        {
            environmentManager.ChangeLight(CurrentStageInfo.FinishPlayerCount, maxCustomerCount);
        }
    }
}

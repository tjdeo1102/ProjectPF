using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class KSD_GameManager : MonoBehaviourPun
{
    [Header("기본 설정")]
    [SerializeField] private int maxCustomerCount;
    [SerializeField] private int currentStageID;
    [SerializeField] private int returnSceneIndex;

    [Header("게임 매니저 구성 요소")]
    [SerializeField] KSD_EnvironmentManager environmentManager;

    [Header("현재 스테이지 정보")]
    public KSD_StageInfo CurrentStageInfo;

    void Start()
    {
        // 각 클라이언트의 맵정보가 동기화되도록 RPC호출
        InitStage();
    }

    /// <summary>
    /// 클라이언트의 맵 정보를 초기화하는 함수
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
            Debug.Log("MapOwnerName가 설정되지 않았습니다.");
        }

        // 방안에 방장이 있는 경우, 방장의 고유 ID정보를 통해, 스테이지 정보를 불러옴
        if (MapOwnerName.IsNullOrEmpty() == false)
        {
            KSD_SaveLoad.Instance.LoadToDatabase(MapOwnerName, currentStageID).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"맵을 로드하는 데 문제가 발생했습니다. 로비로 복귀합니다. \n Error: {task.Exception}");
                    // TODO: 로비로 복귀하는 코드 필요
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
                else
                {
                    if (task.Result == null)
                    {
                        Debug.LogWarning("맵 데이터가 없으므로 새로운 맵 데이터를 생성합니다.");
                        CurrentStageInfo = new KSD_StageInfo();
                        CurrentStageInfo.StageID = currentStageID;
                    }
                    else
                    {
                        Debug.Log("정상적으로 맵 로딩 성공");
                        CurrentStageInfo = task.Result;
                    }
                    UpdateEnvironment();
                }
            });
        }
        else
        {
            Debug.LogError("비정상적인 mapOwnerID. 로비로 복귀합니다.");
            // TODO: 로비로 복귀하는 코드 필요
            PhotonNetwork.LoadLevel(returnSceneIndex);
        }
    }

    [PunRPC]
    private void ChangeFinishPlayerCountRPC(int changeCount)
    {

        CurrentStageInfo.FinishPlayerCount += changeCount;

        if (CurrentStageInfo.FinishPlayerCount >= maxCustomerCount)
        {
            // 스테이지 상승
            CurrentStageInfo.StageLevel++;
            CurrentStageInfo.FinishPlayerCount = 0;
        }
        UpdateEnvironment();
    }

    /// <summary>
    /// 현재 완료된 플레이어의 카운트를 변경하는 함수
    /// </summary>
    /// <param name="changeCount"> 추가되거나 감소될 카운트 </param>
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
                Debug.LogError("맵을 저장하는 데 문제가 발생했습니다. 로비로 복귀합니다.");
                PhotonNetwork.LoadLevel(returnSceneIndex);
            }
            else
            {
                if (task.Result == true)
                {
                    Debug.Log("정상적으로 맵 저장 성공, 로비로 복귀합니다.");
                    PhotonNetwork.LoadLevel(returnSceneIndex);
                }
                else
                {
                    Debug.LogError("맵을 저장하는 데 문제가 발생했습니다. 로비로 복귀합니다.");
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

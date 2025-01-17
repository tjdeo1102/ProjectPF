using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class KSD_AddToolRoomPanel : MonoBehaviour
{
    [Header("계속하기 리스트 설정")]
    [SerializeField] private List<KSD_GameEntryPrefab> entrys;

    private LSY_RoomPanel roomPanel;
    public void Init(LSY_RoomPanel panel)
    {
        roomPanel = panel;
    }    

    public void UpdateList()
    {
        if (KSD_SaveLoad.Instance == null
            || PhotonNetwork.MasterClient == null
            || entrys == null) return;
        
        // 게임 리스트 로드
        KSD_SaveLoad.Instance.LoadGameListToDatabase(PhotonNetwork.MasterClient.NickName)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogWarning($"스테이지 데이터 로드 중 오류 {task.Exception}");
                    return;
                }
                else if (task.IsCompleted)
                {
                    var res = task.Result;
                    if (res != null)
                    {
                        // 날짜 순으로 게임 데이터 정렬
                        res.Sort((a, b) => { return a.StageDate.CompareTo(b.StageDate); });
                    }
                    // 엔트리 개수를 초과한 데이터는 자동으로 제외
                    for (int i = 0; i < entrys.Count; i++)
                    {
                        // 데이터가 없거나
                        // 데이터를 채우고 남은 데이터의 ID는 -1로 초기화
                        if (res == null || res.Count <= i)
                        {
                            entrys[i].StageInfo = new KSD_StageInfo { StageID = -1 };
                        }
                        else
                        {
                            // 해당 엔트리에 맞는 정보로 갱신
                            entrys[i].StageInfo = new KSD_StageInfo
                            {
                                VisitPlayerCount = res[i].VisitPlayerCount,
                                FinishPlayerCount = res[i].FinishPlayerCount,
                                StageDate = res[i].StageDate,
                                StageID = res[i].StageID,
                                StageLevel = res[i].StageLevel,
                                StageMoney = res[i].StageMoney,
                                BuyItems = res[i].BuyItems,
                                ActiveNotes = res[i].ActiveNotes,
                                ActivePerfumes = res[i].ActivePerfumes
                            };
                        }
                        entrys[i].UpdateStageInfo();
                        if (roomPanel != null)
                        {
                            entrys[i].OnClickButtonBind(roomPanel);
                        }
                    }
                }
            });
    }
}

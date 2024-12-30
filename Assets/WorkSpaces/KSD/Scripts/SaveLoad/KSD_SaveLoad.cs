using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class KSD_SaveLoad : MonoBehaviour
{
    public static KSD_SaveLoad Instance { get; private set; }

    private void Awake()
    {
        SetSIngleton();
    }

    void SetSIngleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 데이터베이스에 현재 스테이지 정보들을 저장하는 함수
    /// 플레이어의 ID에 맞는 데이터베이스만 접근가능하도록 설정
    /// 전달받은 현재 스테이지 정보대로 갱신
    /// </summary>
    public async Task<bool> SaveToDatabase(string playerName, KSD_StageInfo stageInfo)
    {
        try
        {
            DatabaseReference root = LSY_BackendManager.Database.RootReference;
            if (LSY_BackendManager.Database == null || root == null)
            {
                Debug.LogError("Database가 null입니다.");
                return false;
            }

            var data = root.Child(playerName).Child(stageInfo.StageID.ToString());
            stageInfo.StageDate = DateTime.Now.ToString();

            // 스테이지 정보를 JSON으로 저장
            string json = JsonUtility.ToJson(stageInfo);

            return await data.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"맵을 저장하는 데 문제가 발생했습니다. {task.Exception}");
                    return false;
                }
                else
                {
                    Debug.Log($"스테이지 {stageInfo.StageID} 정보가 정상적으로 저장 ");
                    return true;
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 저장 중 오류 {e.Message}");
            return false;
        }
    }


    public async Task<List<KSD_StageInfo>> LoadGameListToDatabase(string playerName)
    {
        try
        {
            DatabaseReference root = LSY_BackendManager.Database.RootReference;
            if (LSY_BackendManager.Database == null || root == null)
            {
                Debug.LogError("Database가 null입니다.");
                return null;
            }

            var playerData = root.Child(playerName);
            if (playerData == null)
            {
                Debug.LogError("데이터베이스에 플레이어 ID가 존재하지 않습니다.");
                return null;
            }

            DataSnapshot snapshot = await playerData.GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log($"{playerName}의 게임 데이터 로드 성공");
                var resultList = new List<KSD_StageInfo>();
                foreach (var data in snapshot.Children)
                {
                    resultList.Add(JsonUtility.FromJson<KSD_StageInfo>(data.GetRawJsonValue()));
                }
                return resultList;
            }
            else
            {
                Debug.LogWarning("빈 스테이지 정보 또는 존재하지 않는 스테이지 ID.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"스테이지 데이터 로드 중 오류 {e.Message}");
            return null;
        }
    }


}

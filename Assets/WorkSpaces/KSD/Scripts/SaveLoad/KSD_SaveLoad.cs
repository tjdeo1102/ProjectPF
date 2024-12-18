using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
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
            DatabaseReference root = KSD_BackendManager.Database.RootReference;
            if (root == null)
            {
                Debug.LogError("Database.RootReference가 null입니다.");
                return false;
            }

            var data = root.Child(playerName).Child(stageInfo.StageID.ToString());
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

    public async Task<KSD_StageInfo> LoadToDatabase(string playerName, int stageID)
    {
        try
        {
            DatabaseReference root = KSD_BackendManager.Database.RootReference;
            if (root == null)
            {
                Debug.LogError("Database.RootReference가 null입니다.");
                return null;
            }

            var playerData = root.Child(playerName);
            if (playerData == null)
            {
                Debug.LogError("데이터베이스에 플레이어 ID가 존재하지 않습니다.");
                return null;
            }

            var stageData = playerData.Child(stageID.ToString());
            if (stageData == null)
            {
                Debug.LogError("데이터베이스에 스테이지 ID가 존재하지 않습니다.");
                return null;
            }

            DataSnapshot snapshot = await stageData.GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log($"스테이지 데이터 로드 성공");
                return JsonUtility.FromJson<KSD_StageInfo>(snapshot.GetRawJsonValue());
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

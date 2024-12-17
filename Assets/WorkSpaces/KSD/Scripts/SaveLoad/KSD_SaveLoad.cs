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
    /// �����ͺ��̽��� ���� �������� �������� �����ϴ� �Լ�
    /// �÷��̾��� ID�� �´� �����ͺ��̽��� ���ٰ����ϵ��� ����
    /// ���޹��� ���� �������� ������� ����
    /// </summary>
    public async Task<bool> SaveToDatabase(string playerName, KSD_StageInfo stageInfo)
    {
        try
        {
            DatabaseReference root = KSD_BackendManager.Database.RootReference;
            if (root == null)
            {
                Debug.LogError("Database.RootReference�� null�Դϴ�.");
                return false;
            }

            var data = root.Child(playerName).Child(stageInfo.StageID.ToString());
            // �������� ������ JSON���� ����
            string json = JsonUtility.ToJson(stageInfo);
            return await data.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"���� �����ϴ� �� ������ �߻��߽��ϴ�. {task.Exception}");
                    return false;
                }
                else
                {
                    Debug.Log($"�������� {stageInfo.StageID} ������ ���������� ���� ");
                    return true;
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ���� �� ���� {e.Message}");
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
                Debug.LogError("Database.RootReference�� null�Դϴ�.");
                return null;
            }

            var playerData = root.Child(playerName);
            if (playerData == null)
            {
                Debug.LogError("�����ͺ��̽��� �÷��̾� ID�� �������� �ʽ��ϴ�.");
                return null;
            }

            var stageData = playerData.Child(stageID.ToString());
            if (stageData == null)
            {
                Debug.LogError("�����ͺ��̽��� �������� ID�� �������� �ʽ��ϴ�.");
                return null;
            }

            DataSnapshot snapshot = await stageData.GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log($"�������� ������ �ε� ����");
                return JsonUtility.FromJson<KSD_StageInfo>(snapshot.GetRawJsonValue());
            }
            else
            {
                Debug.LogWarning("�� �������� ���� �Ǵ� �������� �ʴ� �������� ID.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"�������� ������ �ε� �� ���� {e.Message}");
            return null;
        }
    }
}

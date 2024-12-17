using Firebase.Database;
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
    public async Task<bool> TrySaveToDatabase(int playerID, KSD_StageInfo stageInfo)
    {
        DatabaseReference root = BackendManager.Database.RootReference;
        if (root == null)
        {
            Debug.LogError("Database.RootReference�� null�Դϴ�.");
            return false;
        }

        var playerData = root.Child(playerID.ToString());
        if (playerData == null)
        {
            Debug.LogError("�����ͺ��̽��� �÷��̾� ID�� �������� �ʽ��ϴ�.");
            return false;
        }

        try
        {
            // �������� ID �߰�
            await playerData.Child(stageInfo.StageID.ToString()).SetValueAsync(stageInfo.StageID.ToString());

            // �������� ������ JSON���� ����
            string json = JsonUtility.ToJson(stageInfo);
            await playerData.Child(stageInfo.StageID.ToString()).SetRawJsonValueAsync(json);

            Debug.Log($"�������� ������ ���������� ���� {stageInfo.StageID}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ���� �� ���� {e.Message}");
            return false;
        }
    }

    public async Task<KSD_StageInfo> LoadToDatabase(int playerID, int stageID)
    {
        DatabaseReference root = BackendManager.Database.RootReference;
        if (root == null)
        {
            Debug.LogError("Database.RootReference�� null�Դϴ�.");
            return null;
        }

        var playerData = root.Child(playerID.ToString());
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

        try
        {
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
            Debug.LogError($"�������� ������ �ε� �� ���� {e.Message}");
            return null;
        }
    }
}

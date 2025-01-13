using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class KSH_PlayerSaveData
{
    public float AudioVolume;  // 마스터 음량 (0.0 ~ 1.0)
    public float BGMVolume;    // 배경음 음량 (0.0 ~ 1.0)
    public float EffectVolume; // 효과음 음량 (0.0 ~ 1.0)
    //public float InputVolume;  // 입력 음량 (0.0 ~ 1.0)
    //public float OutputVolume; // 출력 음량 (0.0 ~ 1.0)

    public KSH_PlayerSaveData()
    {
        AudioVolume = 0.5f;
        BGMVolume = 0.5f;
        EffectVolume = 0.5f;
        //InputVolume = 0.5f;
        //OutputVolume = 0.5f;
    }
}

public class KSH_PlayerData : MonoBehaviour
{
    public static KSH_PlayerSaveData PlayerSaveData { get; set; }
    private static string saveFilePath => Application.persistentDataPath + "/PlayerData.json";

    private void Awake()
    {
        LoadPlayerData(); // 게임 시작 시 데이터 로드
    }

    private void OnApplicationQuit()
    {
        SavePlayerData(); // 게임 종료 시 데이터 저장
    }

    // 데이터 저장
    public static void SavePlayerData()
    {
        string json = JsonUtility.ToJson(PlayerSaveData); // JSON으로 직렬화
        File.WriteAllText(saveFilePath, json); // 파일로 저장
        Debug.Log($"데이터 저장! {saveFilePath}");
    }

    // 데이터 로드
    public static void LoadPlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath); // JSON 파일 읽기
            PlayerSaveData = JsonUtility.FromJson<KSH_PlayerSaveData>(json); // 역직렬화
            Debug.Log("데이터 불러오기!");
        }
        else
        {
            Debug.LogWarning("세이브 데이터가 없을 때 기본값 사용!");
            PlayerSaveData = new KSH_PlayerSaveData(); // 기본값 사용
        }
    }
}


// 테스트용 설정 저장 불러오기 데이터
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_DryingRackManager : MonoBehaviour
{
    [Header("건조시간")]
    [SerializeField] public int Times;

    public static KSH_DryingRackManager Instance;
    private void Awake()
    {
        // 싱글턴 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 존재하면 현재 게임 오브젝트 삭제
        }
    }
}

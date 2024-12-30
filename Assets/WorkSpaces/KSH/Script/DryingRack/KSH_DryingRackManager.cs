using System;
using UnityEngine;

public class KSH_DryingRackManager : MonoBehaviour
{
    [Header("건조시간")]
    [SerializeField] private int times;

    public static KSH_DryingRackManager Instance;

    public Action<int> OnTimesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int Times
    {
        get => times;
        set
        {
            if (times != value)
            {
                times = value;
                OnTimesChanged?.Invoke(times); // 값 변경 이벤트 호출
            }
        }
    }
}
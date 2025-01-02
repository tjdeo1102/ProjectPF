using System;
using UnityEngine;

public class KSH_DryingRackManager : MonoBehaviour
{
    [Header("건조시간")]
    [SerializeField] private int times;
    [SerializeField] private float targetValue;

    public static KSH_DryingRackManager Instance;

    public Action<int> OnTimesChanged;
    public Action<float> OnTargetValueChanged;

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

    public float TargetValue
    {
        get => targetValue;
        set
        {
            if (targetValue != value)
            {
                targetValue = value;
                OnTargetValueChanged?.Invoke(targetValue); // 값 변경 이벤트 호출
            }
        }
    }
}
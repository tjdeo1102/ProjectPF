using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_EffectManager : MonoBehaviour
{
    public static KSH_EffectManager Instance;

    [Header("Effect")]
    public GameObject[] effectPrefabs; // 이펙트 프리팹 배열
    public float[] effectDurations; // 이펙트마다 지속 시간을 저장할 배열
    private Dictionary<int, Queue<GameObject>> effectPools; // 오브젝트 풀

    // 이펙트 종류를 열거형 표시 추가 가능
    public enum Effect { Fire }
    // 예시, 다른 이펙트 추가 가능

    private void Awake()
    {
        // 싱글턴 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            InitializePools(); // 오브젝트 풀 초기화
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 존재하면 현재 게임 오브젝트 삭제
        }
    }

    // 오브젝트 풀 초기화
    private void InitializePools()
    {
        GameObject effectObject = new GameObject("EffectPlayer");
        effectPools = new Dictionary<int, Queue<GameObject>>();
        effectObject.transform.parent = transform;

        for (int i = 0; i < effectPrefabs.Length; i++)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int j = 0; j < 10; j++) // 각 이펙트에 대해 5개의 인스턴스를 미리 생성
            {
                GameObject obj = Instantiate(effectPrefabs[i]);
                obj.SetActive(false); // 초기에는 비활성화
                obj.transform.parent = effectObject.transform;
                pool.Enqueue(obj);
            }
            effectPools.Add(i, pool);
        }
    }

    // 이펙트를 재생하는 메소드
    public void PlayEffect(Effect effectType, Vector3 position)
    {
        // 효과 유형에 맞는 프리팹 선택
        int effectIndex = (int)effectType;

        // TryGetValue 주어진 두 개의 매개변수로 해당하는 값을 찾을 때 사용
        // 예 TryGetValue(이펙트이름, 이름에 해당하는 이펙트)
        if (effectPools.TryGetValue(effectIndex, out Queue<GameObject> pool))
        {
            GameObject effectInstance;

            // 풀에서 비활성화된 객체를 가져옴
            if (pool.Count > 0)
            {
                effectInstance = pool.Dequeue();
            }
            else
            {
                return; // 더 이상 사용할 수 있는 이펙트가 없으면 종료
            }

            effectInstance.transform.position = position;
            effectInstance.SetActive(true); // 활성화

            // 이펙트의 지속 시간 가져오기
            float duration = effectDurations[effectIndex];

            // 이펙트가 지정된 시간 후 비활성화하고 풀로 반환
            StartCoroutine(ReturnToPool(effectInstance, duration, pool));
        }
    }

    // 일정 시간 후 객체를 비활성화하고 풀로 반환하는 코루틴
    private IEnumerator ReturnToPool(GameObject effectInstance, float duration, Queue<GameObject> pool)
    {
        yield return new WaitForSeconds(duration);
        effectInstance.SetActive(false); // 비활성화
        pool.Enqueue(effectInstance); // 풀에 반환
    }

    // 모든 이펙트를 정지하는 메소드 (예: 게임 종료 시)
    public void StopAllEffects()
    {
        foreach (var pool in effectPools.Values)
        {
            while (pool.Count > 0)
            {
                GameObject effect = pool.Dequeue();
                effect.SetActive(false); // 비활성화
            }
        }
    }
}

// 사용 방법
// KSH_EffectManager.Instance.PlayEffect(KSH_EffectManager.Effect.Fire, mousePosition);
// KSH_EffectManager.Instance.PlayEffect(KSH_EffectManager.이펙트이름, 이펙트 나올 위치)
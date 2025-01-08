using System.Collections;
using UnityEngine;

public class KSH_Elevator : MonoBehaviour
{
    [SerializeField] private float targetFloor; // 엘리베이터가 이동할 목표 층의 높이. Unity 에디터에서 설정 가능
    [SerializeField] private float speed = 2f; // 엘리베이터의 이동 속도. 기본값은 2

    private Vector3 targetPos; // 엘리베이터가 이동할 최종 위치
    private Vector3 startPos; // 엘리베이터의 초기 위치

    private Coroutine elevatorCoroutine; // 현재 실행 중인 엘리베이터 코루틴을 저장할 변수

    private void Start()
    {
        startPos = transform.position; // 초기 위치를 저장
        targetPos = transform.position + Vector3.up * targetFloor; // 목표 위치를 계산 (현재 위치에서 위쪽으로 _targetFloor만큼 이동)
    }

    // 플레이어가 엘리베이터에 진입했을 때 호출되는 함수
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지지 않거나, 스크립트가 비활성화된 경우 리턴
        if (!other.CompareTag("Player") || !enabled) return;

        // 이미 실행 중인 코루틴이 있다면 중지
        if (elevatorCoroutine != null)
            StopCoroutine(elevatorCoroutine);

        // 새 코루틴을 시작하여 엘리베이터에 진입하는 로직 실행
        elevatorCoroutine = StartCoroutine(ElevatorEnter(other.GetComponent<KSH_PailCheck>()));
    }

    // 플레이어가 엘리베이터를 나갈 때 호출되는 함수
    private void OnTriggerExit(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지지 않거나, 스크립트가 비활성화된 경우 리턴
        if (!other.CompareTag("Player") || !enabled) return;

        // 엘리베이터가 복귀를 시작하도록 호출
        StartEixtElevator();
    }

    // 엘리베이터 복귀를 시작하는 함수
    public void StartEixtElevator()
    {
        // 이미 실행 중인 코루틴이 있다면 중지
        if (elevatorCoroutine != null)
            StopCoroutine(elevatorCoroutine);

        // 새 코루틴을 시작하여 엘리베이터 복귀 로직 실행
        elevatorCoroutine = StartCoroutine(ElevatorExit());
    }

    // 엘리베이터가 플레이어와 함께 목표 층으로 올라가는 코루틴
    IEnumerator ElevatorEnter(KSH_PailCheck pailCheck)
    {
        // 엘리베이터 사운드 재생
        // KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Slice);

        while (true)
        {
            // 플레이어가 움직이고 있지 않은 경우에만 엘리베이터 이동
            if (!pailCheck.Iselevator)
            {
                // 엘리베이터를 목표 위치로 이동
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

                // 플레이어 위치를 엘리베이터 위치로 동기화
                pailCheck.gameObject.transform.position = transform.position;

                // 목표 위치에 도달하면 루프 탈출
                if (Mathf.Abs(targetPos.sqrMagnitude - transform.position.sqrMagnitude) < 0.01f)
                    break;
            }
            // 한 프레임 대기
            yield return null;
        }
    }

    // 엘리베이터가 초기 위치로 복귀하는 코루틴
    IEnumerator ElevatorExit()
    {
        while (true)
        {
            // 엘리베이터를 초기 위치로 이동
            transform.position = Vector3.MoveTowards(transform.position, startPos, speed * Time.deltaTime);

            // 초기 위치에 도달하면 루프 탈출
            if (Mathf.Abs(transform.position.sqrMagnitude - startPos.sqrMagnitude) < 0.01f)
                break;

            // 한 프레임 대기
            yield return null;
        }
        // 코루틴 종료
        yield return null;
    }
}
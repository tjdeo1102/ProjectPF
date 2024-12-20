using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WGH_FanTest : MonoBehaviour
{
    [SerializeField] private Transform fan; // 부채의 Transform
    [SerializeField] private Transform fireSource; // 불 파티클의 위치
    [SerializeField] private ParticleSystem fireParticle; // 불 파티클
    public float maxDistance = 5f; // 최대 작동 거리

    private Vector3 lastPosition;
    private float shakeSpeed;       // 부채질 속도
    [SerializeField] private float activeShakeSpeed; // 활성화 되는데 필요한 부채직 속도
    private bool isGrabbed = false; // 부채가 잡혔는지 여부

    void Start()
    {
        lastPosition = fan.position; // 부채 초기 위치 저장
    }

    void Update()
    {
        if (!isGrabbed)  // 부채가 잡혀있지 않으면 Update 중단
            return;
        
        // 부채의 이동 속도 계산
        shakeSpeed = (fan.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = fan.position;
        Debug.Log(shakeSpeed);
        // 부채와 불 사이의 거리 계산
        float distance = Vector3.Distance(fan.position, fireSource.position);

        if (distance <= maxDistance)
        {
            AdjustFire(shakeSpeed); // 거리가 조건에 충족되면 파티클 조정
        }
        else
        {
            ResetFire(); // 거리가 멀어지면 불 파티클을 최소 상태로 유지
        }
    }

    // XR Grab Interactable의 Select Entered 이벤트
    public void OnGrabbed()
    {
        isGrabbed = true; // 부채가 잡힘
    }

    // XR Grab Interactable의 Select Exited 이벤트
    public void OnReleased()
    {
        isGrabbed = false; // 부채가 놓임
        ResetFire(); // 부채를 놓았을 때 파티클 최소 상태로 유지
    }

    void AdjustFire(float speed)
    {
        var emission = fireParticle.emission;
        var main = fireParticle.main;

        if (speed > activeShakeSpeed) // 부채가 흔들릴 때
        {
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 100f, Time.deltaTime);   // 파티클 증가
            main.startSize = Mathf.Lerp(main.startSize.constant, 2f, Time.deltaTime);                   // 크기 증가
        }
        else // 부채가 멈출 때
        {
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 0f, Time.deltaTime);     // 파티클 감소
            main.startSize = Mathf.Lerp(main.startSize.constant, 0.5f, Time.deltaTime);                 // 크기 감소
        }
    }

    void ResetFire()
    {
        var emission = fireParticle.emission;
        var main = fireParticle.main;

        emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 0f, Time.deltaTime);
        main.startSize = Mathf.Lerp(main.startSize.constant, 0.3f, Time.deltaTime);
    }
}

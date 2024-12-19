using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Slices : MonoBehaviour
{
    [SerializeField] private Vector3 sliceNormal = Vector3.up; // 슬라이싱 평면 방향
    [SerializeField] private float sliceCooldown = 0.3f; // 슬라이싱 쿨타임 (초)
    [SerializeField] private float swingThreshold = 2f; // 휘두르기 감지 임계값 (속도)

    private float lastSliceTime = 0f; // 마지막 슬라이싱 시간
    private Vector3 lastPosition; // 마지막 프레임의 위치

    private void Start()
    {
        lastPosition = transform.position; // 초기 위치 저장
    }

    private void OnTriggerEnter(Collider other)
    {
        // 쿨타임 검사: 쿨타임이 지나지 않으면 슬라이싱을 실행하지 않음
        if (Time.time - lastSliceTime < sliceCooldown)
        {
            return;
        }

        // 막대기의 이동 속도(휘두르기 감지)
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > swingThreshold) // 휘두르기 속도 임계값 초과 시에만 실행
        {
            // 충돌한 오브젝트에 Slice 컴포넌트가 있는지 확인
            Slice sliceComponent = other.GetComponent<Slice>();

            if (sliceComponent != null)
            {
                // 슬라이싱 평면의 원점: 막대기(칼)의 현재 위치
                Vector3 sliceOrigin = transform.position;

                // 슬라이싱 평면 방향: 막대기의 진행 방향을 기반으로 설정
                Vector3 sliceDirection = transform.forward;

                // 슬라이싱 실행
                sliceComponent.ComputeSlice(sliceDirection, sliceOrigin);

                // 마지막 슬라이싱 시간 업데이트
                lastSliceTime = Time.time;

                Debug.Log("Object sliced!");
            }
        }

        // 현재 위치를 업데이트
        lastPosition = transform.position;
    }

    private void Update()
    {
        // 프레임마다 현재 위치를 업데이트 (휘두르기 속도 계산용)
        lastPosition = transform.position;
    }
}
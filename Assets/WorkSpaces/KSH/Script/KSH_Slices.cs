using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Slices : MonoBehaviour
{
    [SerializeField] private Vector3 sliceNormal = Vector3.up; // 슬라이싱 평면 방향 (기본값)

    private void OnTriggerEnter(Collider other)
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
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Slices : MonoBehaviour
{
    [SerializeField] private Vector3 sliceNormal = Vector3.up; // �����̽� ��� ���� (�⺻��)

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� Slice ������Ʈ�� �ִ��� Ȯ��
        Slice sliceComponent = other.GetComponent<Slice>();

        if (sliceComponent != null)
        {
            // �����̽� ����� ����: �����(Į)�� ���� ��ġ
            Vector3 sliceOrigin = transform.position;

            // �����̽� ��� ����: ������� ���� ������ ������� ����
            Vector3 sliceDirection = transform.forward;

            // �����̽� ����
            sliceComponent.ComputeSlice(sliceDirection, sliceOrigin);
        }
    }
}
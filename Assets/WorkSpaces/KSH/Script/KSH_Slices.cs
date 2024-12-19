using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Slices : MonoBehaviour
{
    [SerializeField] private Vector3 sliceNormal = Vector3.up; // �����̽� ��� ����
    [SerializeField] private float sliceCooldown = 0.3f; // �����̽� ��Ÿ�� (��)
    [SerializeField] private float swingThreshold = 2f; // �ֵθ��� ���� �Ӱ谪 (�ӵ�)

    private float lastSliceTime = 0f; // ������ �����̽� �ð�
    private Vector3 lastPosition; // ������ �������� ��ġ

    private void Start()
    {
        lastPosition = transform.position; // �ʱ� ��ġ ����
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��Ÿ�� �˻�: ��Ÿ���� ������ ������ �����̽��� �������� ����
        if (Time.time - lastSliceTime < sliceCooldown)
        {
            return;
        }

        // ������� �̵� �ӵ�(�ֵθ��� ����)
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (speed > swingThreshold) // �ֵθ��� �ӵ� �Ӱ谪 �ʰ� �ÿ��� ����
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

                // ������ �����̽� �ð� ������Ʈ
                lastSliceTime = Time.time;

                Debug.Log("Object sliced!");
            }
        }

        // ���� ��ġ�� ������Ʈ
        lastPosition = transform.position;
    }

    private void Update()
    {
        // �����Ӹ��� ���� ��ġ�� ������Ʈ (�ֵθ��� �ӵ� ����)
        lastPosition = transform.position;
    }
}
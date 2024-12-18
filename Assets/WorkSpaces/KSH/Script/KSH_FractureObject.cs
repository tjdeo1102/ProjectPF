using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// FractureObject Ŭ������ Ư�� ������Ʈ�� �浹 �� �ı��ǰ� 
// ���� ȿ���� �����ϸ�, ��Ƽ�÷��̾� ȯ�濡�� �� ������ ����ȭ�ϴ� ������ �մϴ�.
public class KSH_FractureObject : MonoBehaviour
{
    // ���� ������Ʈ�� MeshRenderer
    MeshRenderer meshRenderer;

    // �ڽ� ������Ʈ���� MeshRenderer �迭
    MeshRenderer[] meshRenderers;

    // �ڽ� ������Ʈ���� Rigidbody �迭 (���� ȿ���� ���)
    [SerializeField]Rigidbody[] rigidbodies;

    // ���� ������Ʈ (�ı� �� Ȱ��ȭ�� ������Ʈ)
    [SerializeField] GameObject frags;

    // ���� ������ ������ ���� ũ��
    [SerializeField] float forcePower;

    // ���̵�ƿ� ������ (0.1�� ���)
    WaitForSeconds fadeDelay = new WaitForSeconds(0.1f);

    // Awake: ������Ʈ�� �ڽ� ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        // ���� ������Ʈ�� MeshRenderer�� �����ɴϴ�.
        meshRenderer = GetComponent<MeshRenderer>();

        // ��� �ڽ� ������Ʈ�� MeshRenderer �迭�� �����ɴϴ�.
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);

        // ��� �ڽ� ������Ʈ�� Rigidbody �迭�� �����ɴϴ�.
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

    public void StartFadeOutRPC()
    {
        // ���� MeshRenderer�� ��Ȱ��ȭ
        meshRenderer.enabled = false;

        // ���� ������Ʈ Ȱ��ȭ
        frags.SetActive(true);

        // �� Rigidbody�� ������ �������� ������ �� �߰�
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 ranPos = Random.insideUnitSphere; // ������ ���� ����
            ranPos.y += 0.3f; // �ణ �������� �߰� ��
            rigidbodies[i].AddForce(ranPos * forcePower, ForceMode.Impulse);
        }

        // ���̵�ƿ� ����
        StartCoroutine(FadeOut());
    }

    // ���̵�ƿ� ȿ���� ó���ϴ� �ڷ�ƾ
    IEnumerator FadeOut()
    {
        float alpha = 1; // ���� ���� ��
        Color color = meshRenderers[0].material.color; // ù ��° Material�� �ʱ� ����

        // ���� ���� 0�� �� ������ �ݺ�
        while (alpha > 0)
        {
            color.a = alpha; // ���� ���� ������Ʈ

            // ��� �ڽ� MeshRenderer�� Material ������ ������Ʈ
            foreach (MeshRenderer renderer in meshRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = color;
                }
            }

            alpha -= 0.04f; // ���� ���� ����
            yield return fadeDelay; // ���
        }
    }
}
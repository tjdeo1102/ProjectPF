using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_DispensorLiquid : MonoBehaviour
{
    public ParticleSystem particleSystemLiquid;
    public float fillAmount = 1f;  
    public MeshRenderer MeshRenderer;

    MaterialPropertyBlock m_MaterialPropertyBlock;

    public Color potionColor;
    public Color linePotionColor;

    private Coroutine pouringliquidRoutine;
    private float totalPourTime = 5f;
    private float pourAmountPerSecond = 0.02f;
    private float totalPourAmount = 0.1f;  // 총 줄어야 할 액체 양

    void OnEnable()
    {
        particleSystemLiquid.Stop();

        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pouringliquidRoutine == null)
            {
                Debug.Log("플레이어와 부딪힘");
                pouringliquidRoutine = StartCoroutine(PouringliquidRoutine());
            }
        }
    }

    IEnumerator PouringliquidRoutine()
    {
        float startTime = Time.time;

        float totalAmountToPour = totalPourAmount;

        while (fillAmount > 0f && (Time.time - startTime) < totalPourTime)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            float amountToPourThisFrame = pourAmountPerSecond * Time.deltaTime;

            fillAmount -= amountToPourThisFrame;

            fillAmount = Mathf.Max(fillAmount, 0f);

            MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

            RaycastHit[] hits = Physics.RaycastAll(particleSystemLiquid.transform.position, Vector3.down, 50.0f, ~0, QueryTriggerInteraction.Collide);

            int receiverCount = 0;
            LSY_PotionReceiver[] receivers = new LSY_PotionReceiver[hits.Length];

            foreach (RaycastHit hit in hits)
            {
                LSY_PotionReceiver receiver = hit.collider.GetComponent<LSY_PotionReceiver>();
                if (receiver != null)
                {
                    receivers[receiverCount] = receiver;
                    receiverCount++;
                }
            }

            if (receiverCount == 2)
            {
                Debug.Log("두 개의 PotionReceiver를 찾음");

                LSY_PotionReceiver receiver = receivers[0];
                receiver.ReceivePotion(potionColor, linePotionColor);
            }
            else
            {
                Debug.Log("PotionReceiver가 두 개 이하");
            }

            yield return null;
        }

        // 반올림하여 소수점 두 번째 자리까지
        fillAmount = Mathf.Round(fillAmount * 10f) / 10f;

        MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        if (fillAmount <= 0f || (Time.time - startTime) >= totalPourTime)
        {
            particleSystemLiquid.Stop();
        }

        pouringliquidRoutine = null;
    }



}

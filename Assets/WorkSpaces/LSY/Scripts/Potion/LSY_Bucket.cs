using System.Collections;
using UnityEngine;

public class LSY_Bucket : MonoBehaviour
{
    static int NextFreeUniqueId = 3000;

    public ParticleSystem particleSystemLiquid;
    public float fillAmount = 0.8f;
    public MeshRenderer MeshRenderer;
    public GameObject fillGameObject;

    MaterialPropertyBlock m_MaterialPropertyBlock;
    Rigidbody m_RbPotion;

    int m_UniqueId;
    bool m_Breakable;
    float m_StartingFillAmount;

    public float targetFillAmount = 1f;
    private bool isFilling = false;

    public Color potionColor;
    public Color linePotionColor;

    void OnEnable()
    {
        particleSystemLiquid.Stop();
        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        m_RbPotion = GetComponent<Rigidbody>();
        m_StartingFillAmount = fillAmount;
        m_Breakable = true;
    }

    void Start()
    {
        m_UniqueId = NextFreeUniqueId++;
    }

    void Update()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0 )
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            fillAmount -= 0.1f * Time.deltaTime;


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


            MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
        }
        else
        {
            particleSystemLiquid.Stop();
        }

        if (fillAmount < 0)
        {
            fillAmount = 0;
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            fillGameObject.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cauldron") && !isFilling)
        {
            fillGameObject.gameObject.SetActive(true);

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cauldron") && !isFilling)
        {
            StartCoroutine(FillBucket());
        }
    }

    private IEnumerator FillBucket()
    {
        isFilling = true;  
        float startTime = Time.time;  

        float initialFillAmount = fillAmount;  

        while (Time.time - startTime < 3f)
        {
            fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, (Time.time - startTime) / 3f);
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            yield return null;  
        }

        fillAmount = targetFillAmount;
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
        isFilling = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LSY_Bucket : MonoBehaviour
{
    static int NextFreeUniqueId = 3000;

    public ParticleSystem particleSystemLiquid;
    public float fillAmount = 0.8f;
    public MeshRenderer MeshRenderer;
    public GameObject fillGameObject;
    [SerializeField] public PerfumeNoteName currentPerfumeNote;
    //[SerializeField] public List<LSY_ColorList> colorLists = new ();

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

        //GameObject particle = Instantiate(colorLists[0].bucketParticle);
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
            LSY_DispensorReceiver[] receivers = new LSY_DispensorReceiver[hits.Length];

            foreach (RaycastHit hit in hits)
            {
                LSY_DispensorReceiver receiver = hit.collider.GetComponent<LSY_DispensorReceiver>();

                if (receiver != null)
                {
                    receivers[receiverCount] = receiver;
                    receiverCount++;

                    if (receiver.currentPerfumeNote != null && receiver.currentPerfumeNote != currentPerfumeNote)
                    {
                        Debug.Log("같은 향이 아닙니다");
                        Debug.Log(currentPerfumeNote);
                        Debug.Log(receiver.currentPerfumeNote);
                        return;
                    }
                }
            }


            if (receiverCount == 2)
            {
                Debug.Log("두 개의 DispensorReceiver를 찾음");

                LSY_DispensorReceiver receiver = receivers[0];
                receiver.ReceivePotion(potionColor, linePotionColor);

            }
            else
            {
                Debug.Log("DispensorReceiver가 두 개 이하");
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
            if (collision.gameObject.GetComponent<KSD_CauldronController>() == null) return;

            currentPerfumeNote = collision.gameObject.GetComponent<KSD_CauldronController>().ResultNoteInfo.Name;


            fillGameObject.gameObject.SetActive(true);
            Debug.Log("양동이 참");

            if (fillBucket == null)
            {
                fillBucket = StartCoroutine(FillBucket());
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cauldron"))
        {
            if (fillBucket != null)
            {
                StopCoroutine(fillBucket);
                isFilling = false;
                fillBucket = null;
            }
        }
    }

    Coroutine fillBucket;
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

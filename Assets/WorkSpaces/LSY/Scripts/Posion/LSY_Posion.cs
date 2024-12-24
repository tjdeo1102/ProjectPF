using UnityEngine;

public class LSY_Posion : MonoBehaviour
{
    static int NextFreeUniqueId = 3000;

    public GameObject plugObj;
    public ParticleSystem particleSystemLiquid;
    public ParticleSystem particleSystemSplash;
    public float fillAmount = 0.8f;
    public GameObject popVFX;
    public MeshRenderer MeshRenderer;
    public GameObject SmashedObject;

    bool m_PlugIn = true;
    Rigidbody m_PlugRb;
    MaterialPropertyBlock m_MaterialPropertyBlock;
    Rigidbody m_RbPotion;

    int m_UniqueId;
    bool m_Breakable;
    float m_StartingFillAmount;

    public Color potionColor;
    public Color linePotionColor;

    void OnEnable()
    {
        particleSystemLiquid.Stop();
        if (particleSystemSplash)
            particleSystemSplash.Stop();

        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        m_PlugRb = plugObj.GetComponent<Rigidbody>();
        popVFX.SetActive(false);

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
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0 && m_PlugIn == false)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            fillAmount -= 0.1f * Time.deltaTime;

            Debug.DrawRay(particleSystemLiquid.transform.position, Vector3.down, Color.red);
            RaycastHit[] hits = Physics.RaycastAll(particleSystemLiquid.transform.position, Vector3.down, 50.0f, ~0, QueryTriggerInteraction.Collide);

            int receiverCount = 0;
            LSY_PosionReceiver[] receivers = new LSY_PosionReceiver[hits.Length];

            foreach (RaycastHit hit in hits)
            {
                LSY_PosionReceiver receiver = hit.collider.GetComponent<LSY_PosionReceiver>();
                if (receiver != null)
                {
                    receivers[receiverCount] = receiver;
                    receiverCount++;
                }
            }

            if (receiverCount == 2)
            {
                Debug.Log("두 개의 PotionReceiver를 찾음");

                LSY_PosionReceiver receiver = receivers[0];
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
    }

    public void PlugOff()
    {
        if (m_PlugIn)
        {
            m_PlugIn = false;
            m_PlugRb.transform.SetParent(null);
            m_PlugRb.isKinematic = false;
            m_PlugRb.AddRelativeForce(new Vector3(0, 0, 120));
        }
    }

    public void ToggleBreakable(bool breakable)
    {
        m_Breakable = breakable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Breakable && m_RbPotion.velocity.magnitude > 1.35)
        {

            if (m_PlugIn)
            {
                m_PlugRb.isKinematic = false;
                plugObj.transform.parent = null;

                Collider c;
                if (plugObj.TryGetComponent(out c))
                    c.enabled = true;

                Destroy(plugObj, 4.0f);
            }

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            if (particleSystemSplash != null)
            {
                particleSystemSplash.gameObject.SetActive(true);
                if (fillAmount > 0)
                {
                    particleSystemSplash.Play();
                }
            }

            SmashedObject.SetActive(true);

            Rigidbody[] rbs = SmashedObject.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.AddExplosionForce(100.0f, SmashedObject.transform.position, 2.0f, 15.0F);
            }

            Destroy(SmashedObject, 4.0f);
            Destroy(this);
        }
    }
}

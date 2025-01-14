using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_LiquidMaterialBottle : MonoBehaviour
{
    static private int NextFreeUniqueId = 3000;

    [Header("기본 설정")]
    [Range(0f, 1f), SerializeField] private float maxFillThreshold;            //최대치로 바뀌는 임계치
    public KSD_PerfumeMaterialInfo Info;
    public float fillAmount;

    [Header("참조 설정")]
    [SerializeField] private ParticleSystem particleSystemLiquid;
    [SerializeField] private ParticleSystem particleSystemSplash;
    public MeshRenderer LiquidRenderer;

    PerfumeMaterialName lastName;

    MaterialPropertyBlock m_MaterialPropertyBlock;
    Rigidbody m_RbPotion;

    int m_UniqueId;
    bool m_Breakable;

    public Color potionColor;
    public Color linePotionColor;

    void OnEnable()
    {
        particleSystemLiquid.Stop();
        if (particleSystemSplash)
            particleSystemSplash.Stop();

        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        LiquidRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        Info = new KSD_PerfumeMaterialInfo();
        Info.Name = PerfumeMaterialName.Null;
        Info.Type = PerfumeMaterialType.Null;
        Info.State = PerfumeMaterialState.Null;
        lastName = PerfumeMaterialName.Null;

        fillAmount = 0f;

        m_RbPotion = GetComponent<Rigidbody>();
        m_Breakable = false;
    }

    void Start()
    {
        m_UniqueId = NextFreeUniqueId++;
    }

    private void Update()
    {
        if (Info.Name != lastName
            && ( Info.Name == PerfumeMaterialName.Cherry
            || Info.Name == PerfumeMaterialName.Blueberry
            || Info.Name == PerfumeMaterialName.Lemon
            || Info.Name == PerfumeMaterialName.Grapefruit ))
        {
            potionColor = Info.GetColorByName(Info.Name);
            linePotionColor = Info.GetColorByName(Info.Name);
            lastName = Info.Name;
        }

        UpdateDropLiquid();

        // 변수에 따라 항상 업데이트
        LiquidRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);
        LiquidRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }
    private void ResetBottle()
    {
        Info.Name = PerfumeMaterialName.Null;
        Info.State = PerfumeMaterialState.Null;
        Info.Type = PerfumeMaterialType.Null;
    }

    void UpdateDropLiquid()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0)
        {
            if (particleSystemLiquid.isStopped)
            {
                // 쏟는 액체의 색 변경
                var mat = particleSystemLiquid.GetComponent<ParticleSystemRenderer>().material;
                mat.SetColor("_BaseColor", potionColor);
                mat.SetColor("_EmissionColor", potionColor);
                // 깨지는 액체의 색 변경
                if (m_Breakable)
                {
                    var mat2 = particleSystemSplash.GetComponent<ParticleSystemRenderer>().material;
                    mat2.SetColor("_BaseColor", potionColor);
                    mat2.SetColor("_EmissionColor", potionColor);
                }

                particleSystemLiquid.Play();
            }

            var delta = 0.1f * Time.deltaTime;
            fillAmount -= delta;
            if (fillAmount < 0)
            {
                fillAmount = 0;
                ResetBottle();
            }
            Info.MaterialAmount = fillAmount;

            Debug.DrawRay(particleSystemLiquid.transform.position, Vector3.down, Color.red);

            // 자기자신 임시로 레이캐스트 제외되는 레이어로 바꾸기
            int tempLayer = gameObject.layer;
            gameObject.layer = 8;
            if (Physics.Raycast(particleSystemLiquid.transform.position, Vector3.down, out var hit, 50.0f, ~(1 << 8), QueryTriggerInteraction.Collide))
            {
                if (hit.collider.transform.parent.TryGetComponent<KSD_ConcentrateBottle>(out var receiver))
                {
                    receiver.ReceiveLiquidMaterial(Info, delta);
                    Debug.Log("받을 KSD_ConcentrateBottle를 찾음");
                }
                else if (hit.collider.transform.parent.TryGetComponent<KSD_LiquidMaterialBottle>(out var receiver2))
                {
                    receiver2.ReceiveLiquidMaterial(Info, delta);
                    Debug.Log("받을 KSD_LiquidMaterialBottle를 찾음");
                }
                else
                {
                    Debug.Log($"PotionReceiver를 찾지 못함 {hit.transform.name}");
                }
            }
            gameObject.layer = tempLayer;
        }
        else particleSystemLiquid.Stop();
    }

    public void ToggleBreakable(bool breakable)
    {
        m_Breakable = breakable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_Breakable && m_RbPotion.velocity.magnitude > 1.35)
        {
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
            Destroy(this);
        }
    }

    public void ReceiveLiquidMaterial(KSD_PerfumeMaterialInfo mat, float getAmount)
    {
        // 만약, 현재 병에 이름이 다른 재료로 채워진 경우에는 리턴
        if (Info.Name != PerfumeMaterialName.Null && Info.Name != mat.Name) return;

        // 같은 재료인 경우거나 병이 비워진 경우에만 정보 업데이트
        Info.Name = mat.Name;
        Info.State = mat.State;
        Info.Type = mat.Type;

        fillAmount += getAmount;
        if (fillAmount > maxFillThreshold) fillAmount = 1f;
        Info.MaterialAmount = fillAmount;
    }
}

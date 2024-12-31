using UnityEngine;

public class LSY_DispensorReceiver : MonoBehaviour
{
    public LSY_DispensorLiquid dispensorLiquid;
    public float maxLiquidFill = 1.0f;
    public float fillAmount = 0.0f;
    private float m_StartingFillAmount;
    public MeshRenderer liquidMeshRenderer;
    private MaterialPropertyBlock m_MaterialPropertyBlock;
    [SerializeField] public PerfumeNoteName currentPerfumeNote;

    void Start()
    {
        currentPerfumeNote = PerfumeNoteName.Floral;
        if (liquidMeshRenderer == null)
        {
            return;
        }

        if (m_MaterialPropertyBlock == null)
        {
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
        }

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    public void ReceivePotion(Color potionColor, Color linePotionColor)
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.1f * Time.deltaTime;

            if (m_MaterialPropertyBlock != null)
            {
                m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
                m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
                m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);
                if (liquidMeshRenderer != null)
                    liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
                dispensorLiquid.fillAmount = fillAmount;
            }

            Debug.Log($"현재 채워진 양: {fillAmount * 100}%");
        }
    }
}

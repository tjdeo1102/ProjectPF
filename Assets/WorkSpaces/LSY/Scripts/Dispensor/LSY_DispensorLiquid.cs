using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;

public class LSY_DispensorLiquid : MonoBehaviourPun
{
    [Header("디스펜서 핸들 애니매이터")]
    [SerializeField] Animator animator;

    [Header("액체 붓는 파티클")]
    public ParticleSystem particleSystemLiquid;

    [Header("최대 액체 양")]
    public float maxLiquidFill = 1.0f;

    [Header("액체 양")]
    public float fillAmount;

    [Header("디스펜서 액체 렌더러")]
    public MeshRenderer MeshRenderer;

    [Header("디스펜서 Info")]
    public LSY_DispensorInfo dispensorInfo;

    Color liquidColor;
    Color liquidLineColor;

    private Coroutine pouringliquidRoutine;
    private float totalPourTime = 1f;
    private float pourAmountPerSecond = 0.1f;
    private float totalPourAmount = 0.1f;  // 총 줄어야 할 액체 양
    MaterialPropertyBlock m_MaterialPropertyBlock;

    private bool isOnCooldown = false;
    private float cooldownTime = 2f;  
    private float cooldownTimer = 0f;

    void Start()
    {
        particleSystemLiquid.Stop();

        liquidColor = dispensorInfo.liquidColor;
        liquidLineColor = dispensorInfo.liquidLineColor;

        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", liquidColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", liquidLineColor);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }


    [PunRPC]
    public void OnSelectEnter()
    {
        if (!isOnCooldown)
        {
            PouringLiquid();
        }
    }

    public void PouringLiquid()
    {
        if (pouringliquidRoutine == null)
        {
            if (fillAmount < 0.1f)
            {
                Debug.Log("한번 나올 양이 부족함");
                return;
            }

            if (isOnCooldown)
            {
                Debug.Log("쿨다운 중입니다. 2초 후에 다시 시도해주세요.");
                return;
            }

            animator.SetTrigger("HandleOn");
            pouringliquidRoutine = StartCoroutine(PouringliquidRoutine());

            StartCooldown();
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;

            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("OnSelectEnter", RpcTarget.AllViaServer);
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
                if (potionReceiverRoutine == null)
                {
                    potionReceiverRoutine = StartCoroutine(PotionReceiverRoutine(receiver));
                }
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

        animator.SetTrigger("HandleOff");
        animator.SetTrigger("HandleIdle");
        pouringliquidRoutine = null;
    }

    Coroutine potionReceiverRoutine;
    IEnumerator PotionReceiverRoutine(LSY_PotionReceiver potionReceiver)
    {
        potionReceiver.ReceivePotion(liquidColor, liquidLineColor, dispensorInfo.noteName);
        yield return new WaitForSeconds(0.1f);
        potionReceiverRoutine = null;
    }

    [PunRPC]
    public void ReceiveLiquid()
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.1f * Time.deltaTime;

            if (m_MaterialPropertyBlock != null)
            {
                m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
                MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            }

            Debug.Log($"현재 채워진 양: {fillAmount * 100}%");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fillAmount);
            stream.SendNext(dispensorInfo);
        }
        else
        {
            fillAmount = (float)stream.ReceiveNext();
            dispensorInfo = (LSY_DispensorInfo)stream.ReceiveNext();
        }
    }
}

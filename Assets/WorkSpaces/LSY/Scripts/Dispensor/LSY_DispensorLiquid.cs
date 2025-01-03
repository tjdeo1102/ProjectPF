using Photon.Pun;
using System.Collections;
using UnityEngine;

public class LSY_DispensorLiquid : MonoBehaviourPun, IPunObservable
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

    bool liquidOn = false;
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
            photonView.RPC("PouringLiquid", RpcTarget.All);
        }
    }

    [PunRPC]
    public void PouringLiquid()
    {
        if (!photonView.IsMine) return;

        if (pouringliquidRoutine == null)
        {
            if (fillAmount < 0.1f)
            {
                Debug.Log("한번 나올 양이 부족함");
                return;
            }

            if (isOnCooldown)
            {
                Debug.Log("2초 후에 다시 시도해주세요.");
                return;
            }

            animator.SetTrigger("HandleOn");
            if (liquidOn == false)
                pouringliquidRoutine = StartCoroutine(PouringliquidRoutine());

            photonView.RPC("StartCooldown", RpcTarget.All);
        }
    }

    [PunRPC]
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
                liquidOn = false;

            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("OnSelectEnter", RpcTarget.AllViaServer);
        }

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    public void OnSelectedEnter()
    {
        photonView.RPC("OnSelectEnter", RpcTarget.AllViaServer);
    }


    IEnumerator PouringliquidRoutine()
    {
        liquidOn = true;

        double startTime = PhotonNetwork.Time;

        float totalAmountToPour = totalPourAmount;

        while (fillAmount > 0f && (PhotonNetwork.Time - startTime) < totalPourTime)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
                photonView.RPC("PlayParticle", RpcTarget.Others);
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

            if (receiverCount >= 2)
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

        particleSystemLiquid.Stop();
        photonView.RPC("StopParticle", RpcTarget.Others);

        animator.SetTrigger("HandleOff");
        animator.SetTrigger("HandleIdle");
        pouringliquidRoutine = null;
    }

    [PunRPC]
    public void StopParticle()
    {
        particleSystemLiquid.Stop();
    }

    [PunRPC]
    public void PlayParticle()
    {
        particleSystemLiquid.Play();
    }

    Coroutine potionReceiverRoutine;
    IEnumerator PotionReceiverRoutine(LSY_PotionReceiver potionReceiver)
    {

        float[] potionColorArray = new float[] { liquidColor.r, liquidColor.g, liquidColor.b, liquidColor.a };
        float[] linePotionColorArray = new float[] { liquidLineColor.r, liquidLineColor.g, liquidLineColor.b, liquidLineColor.a };

        for (int i = 0; i < 10; i++)
        {
            potionReceiver.photonView.RPC("ReceivePotion", RpcTarget.All, potionColorArray, linePotionColorArray, dispensorInfo.noteName, dispensorInfo.state);
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(1);
        potionReceiverRoutine = null;
    }

    [PunRPC]
    public void FillAmountUpdate()
    {
        float amountToPourThisFrame = pourAmountPerSecond * Time.deltaTime;

        fillAmount -= amountToPourThisFrame;

        fillAmount = Mathf.Max(fillAmount, 0f);

        MeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    [PunRPC]
    public void ReceiveLiquid()
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.07f * Time.deltaTime;

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
        }
        else
        {
            fillAmount = (float)stream.ReceiveNext();
        }
    }

}

using Photon.Pun;
using System.Collections;
using UnityEngine;

public class LSY_DispensorLiquid : MonoBehaviourPun, IPunObservable
{
    [Header("디스펜서 뚜껑 애니매이터")]
    [SerializeField] Animator litAnimator;

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

    [Header("디스펜서 UI")]
    public GameObject despensorUI;
    bool onDespensorUI = false;
    public E_WGH_NoteType noteType;
    public LSY_DespensorLever LSY_DespensorLever;
    public WGH_NoteUIControl noteUIControl;


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
    public bool isLitOpen = false;
    void Start()
    {
        noteUIControl = GameObject.FindWithTag("TestNote").GetComponent<WGH_NoteUIControl>();

        particleSystemLiquid.Stop();
        despensorUI.SetActive(false);

        liquidColor = dispensorInfo.liquidColor;
        liquidLineColor = dispensorInfo.liquidLineColor;

        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", liquidColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", liquidLineColor);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
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

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

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
        //if (!photonView.IsMine) return;

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

            if (liquidOn == false)
                pouringliquidRoutine = StartCoroutine(PouringliquidRoutine());

            StartCooldown();
        }
    }

    [PunRPC]
    public void RPC_LitAnimation(string name, bool on)
    {
        litAnimator.SetTrigger(name);
        isLitOpen = on;
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
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
                LSY_PotionReceiver receiver = receivers[0];
                if (potionReceiverRoutine == null)
                {
                    KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.PourWater);
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

        LSY_DespensorLever.StartRoutine();
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
            potionReceiver.ReceivePotion(potionColorArray, linePotionColorArray, dispensorInfo.noteName, dispensorInfo.state);
            yield return new WaitForSeconds(0.1f);
        }

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
        if (!isLitOpen) 
        {
            Debug.Log("뚜껑 닫혀있음");
            return; 
        }

        if (onDespensorUI == false)
        {
            despensorUI.SetActive(true);
            onDespensorUI = true;
            noteUIControl.OnUI(noteType);
        }

        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.04f * Time.deltaTime;

            if (m_MaterialPropertyBlock != null)
            {
                m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
                MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            }

            //Debug.Log($"현재 채워진 양: {fillAmount * 100}%");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fillAmount);
            stream.SendNext(isLitOpen);
            stream.SendNext(liquidOn);
        }
        else
        {
            fillAmount = (float)stream.ReceiveNext();
            isLitOpen = (bool)stream.ReceiveNext();
            liquidOn = (bool)stream.ReceiveNext();
        }
    }

}

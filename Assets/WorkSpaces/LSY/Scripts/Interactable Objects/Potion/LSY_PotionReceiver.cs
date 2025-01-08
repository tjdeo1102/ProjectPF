using DG.Tweening.Plugins.Options;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_PotionReceiver : MonoBehaviourPun, IPunObservable
{
    [Header("최대 포션 양")]
    public float maxLiquidFill = 1.0f;

    [Header("포션 양")]
    public float fillAmount = 0.0f;

    [Header("액체 렌더러")]
    public MeshRenderer liquidMeshRenderer;

    [Header("조합 전 노트 리스트")]
    public List<KSD_PerfumeNoteInfo> perfumeNoteInfoLists;

    [Header("조합 후 향수 정보")]
    public KSD_PerfumeInfo resInfo;
    public E_WGH_PerfumeType perfumeName;
    public E_BottleType e_BottleType;

    [Header("액체 붓는 파티클")]
    public ParticleSystem particleSystemLiquid;

    [Header("Shake")]
    [SerializeField] private float shakeTimer;
    [SerializeField] private float distancePerFrame;
    private float lastShakeTime;
    private float deadTime = 1f;

    [Header("Splash")]
    public GameObject potion;
    public ParticleSystem particleSystemSplash;

    int receiveCount = 0;

    private Color potionColor;
    private Color linePotionColor;

    public bool m_Breakable = true;
    Rigidbody m_RbPotion;

    bool perfumeClear = false;
    private MaterialPropertyBlock m_MaterialPropertyBlock;

    private bool IsActiveShake;
    private Vector3 lastSpoonPosition;

    private Coroutine shakeRoutine;

    void Start()
    {
        resInfo.Name = PerfumeName.Null;
        perfumeNoteInfoLists = new List<KSD_PerfumeNoteInfo>();
        m_RbPotion = GetComponent<Rigidbody>();

        if (liquidMeshRenderer == null)
        {
            return;
        }

        if (m_MaterialPropertyBlock == null)
        {
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
        }
    }

    private void Update()
    {
        UpdateDropLiquid();
        CheckShake();

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

    }

    private void CheckShake()
    {
        var pos = transform.position;
        bool isShakingNow = Vector3.Distance(lastSpoonPosition, pos) > distancePerFrame;

        if (isShakingNow)
        {
            lastShakeTime = Time.time;
            IsActiveShake = true;

            if (shakeRoutine == null)
                shakeRoutine = StartCoroutine(ShakeRoutine());
        }
        else
        {
            if (Time.time - lastShakeTime > deadTime)
            {
                IsActiveShake = false;

                if (shakeRoutine != null)
                {
                    StopCoroutine(shakeRoutine);
                    shakeRoutine = null;
                }
            }
        }

        lastSpoonPosition = pos; 
    }

    IEnumerator ShakeRoutine()
    {
        yield return new WaitForSeconds(shakeTimer);
        shakeRoutine = null;
        photonView.RPC("PerfumeDone", RpcTarget.All);
    }

    [PunRPC]
    public void PerfumeDone()
    {
        if (perfumeNoteInfoLists.Count < 1 || resInfo.Name != PerfumeName.Null) return;

        if (KSD_PerfumeManager.Instance.IsValidPerfumeRecipe(perfumeNoteInfoLists, out var res))
        {
            resInfo = new KSD_PerfumeInfo();
            resInfo.Name = res.Name;

            potionColor = resInfo.GetColorByName(res.Name);
            linePotionColor = resInfo.GetColorByName(res.Name);

            m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
            m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

            liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

            int perfumeValue = (int)resInfo.Name;

            perfumeName = (E_WGH_PerfumeType)perfumeValue;
        }
        else
        {
            photonView.RPC("FusionFail", RpcTarget.All);
        }

        perfumeNoteInfoLists.Clear();
        shakeRoutine = null;
        Debug.Log("흔들기완료");
        m_RbPotion.velocity = Vector3.zero;
    }

    [PunRPC]
    private void FusionFail()
    {
        perfumeClear = true;
        resInfo = new KSD_PerfumeInfo();
        resInfo.Name = PerfumeName.Null;
        perfumeName = E_WGH_PerfumeType.NONE;

        potionColor = Color.black;
        linePotionColor = Color.black;

        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

        liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    public void ReceivePotion(float[] potionColor, float[] linePotionColor, PerfumeNoteName perfumeNoteName, PerfumeNoteState state)
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.02f;
            receiveCount++;

            Color colorPotion = new Color(potionColor[0], potionColor[1], potionColor[2], potionColor[3]);
            Color colorLinePotion = new Color(linePotionColor[0], linePotionColor[1], linePotionColor[2], linePotionColor[3]);

            m_MaterialPropertyBlock.SetColor("Color_E3091B1A", colorPotion);
            m_MaterialPropertyBlock.SetColor("Color_FDA61C50", colorLinePotion);
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

            if (receiveCount == 7)
            {
                foreach (var potionInfo in perfumeNoteInfoLists)
                {
                    if (potionInfo.Name == perfumeNoteName)
                    {
                        potionInfo.NoteCount += 1;
                        StartCoroutine(ResetCountRoutine());
                        return;
                    }
                }

                KSD_PerfumeNoteInfo noteInfo = new KSD_PerfumeNoteInfo();
                noteInfo.Name = perfumeNoteName;
                noteInfo.State = state;
                noteInfo.NoteCount += 1;
                perfumeNoteInfoLists.Add(noteInfo);

                StartCoroutine(ResetCountRoutine());
            }
        }
    }

    IEnumerator ResetCountRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        fillAmount = Mathf.Round(fillAmount * 10f) / 10f;
        receiveCount = 0;
    }

    void UpdateDropLiquid()
    {
        if (!perfumeClear) { return; }

        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            DecreaseLiquid();
        }
        else
        {
            particleSystemLiquid.Stop();
        }
    }

    void DecreaseLiquid()
    {
        var delta = 0.3f * Time.deltaTime;
        fillAmount -= delta;

        if (fillAmount <= 0)
        {
            fillAmount = 0;
            photonView.RPC("ResetBottle", RpcTarget.All);
            photonView.RPC("PlayLiquidParticle", RpcTarget.All, false);
        }
    }


    [PunRPC]
    public void ResetBottle()
    {
        perfumeClear = false;
        fillAmount = 0f; 
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fillAmount);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_Breakable);
        }
        else
        {
            fillAmount = (float)stream.ReceiveNext();
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            m_Breakable = (bool)stream.ReceiveNext();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_RbPotion == null) return;

        if (m_RbPotion.velocity.magnitude > 1.35 && m_Breakable)
        {
            if (particleSystemSplash != null)
            {
                fillAmount = 0f;
                particleSystemSplash.gameObject.SetActive(true);
                photonView.RPC("PlaySplashParticle", RpcTarget.All);
            }
            photonView.RPC("DestroyPotion", RpcTarget.All);
        }
    }

    [PunRPC]
    public void PlaySplashParticle()
    {
        particleSystemSplash.Play();
    }

    [PunRPC]
    public void PlayLiquidParticle(bool on)
    {
        if (on)
        {
            particleSystemLiquid.Play();
        }
        else
        {
            particleSystemLiquid.Stop();
        }

    }

    [PunRPC]
    public void DestroyPotion()
    {
        Destroy(potion);
        Destroy(gameObject, 3f);
    }

    public void ToggleBreakable(bool breakable)
    {
        m_Breakable = breakable;
    }

}

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

    [Header("액체 붓는 파티클")]
    public ParticleSystem particleSystemLiquid;

    [SerializeField] private float shakeTimer;
    [SerializeField] private float distancePerFrame;

    public int receiveCount = 0;

    private Color potionColor;
    private Color linePotionColor;

    bool perfumeClear = false;
    private MaterialPropertyBlock m_MaterialPropertyBlock;

    private bool IsActiveShake;
    private Vector3 lastSpoonPosition;

    private Coroutine shakeRoutine;

    void Start()
    {
        resInfo.Name = PerfumeName.Null;
        perfumeNoteInfoLists = new List<KSD_PerfumeNoteInfo>();

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

    private void Update()
    {
        photonView.RPC("CheckShake", RpcTarget.All);

        photonView.RPC("UpdateDropLiquid", RpcTarget.All);
    }

    [PunRPC]
    private void CheckShake()
    {
        var pos = transform.position;
        IsActiveShake = Vector3.Distance(lastSpoonPosition, pos) > distancePerFrame;
        lastSpoonPosition = transform.position;
        Debug.Log("checkshake2");
        if (IsActiveShake)
        {
            Debug.Log("액티베이트");
            if (shakeRoutine == null)
                photonView.RPC("PerfumeDone", RpcTarget.All);
        }
        else
        {
            Debug.Log("노액티베이트");
            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
                shakeRoutine = null;
            }
        }
    }

    IEnumerator ShakeRoutine()
    {
        Debug.Log("checkshake3");
        yield return new WaitForSeconds(shakeTimer);
        Debug.Log("checkshake3.5");
        photonView.RPC("PerfumeDone", RpcTarget.All);
    }

    [PunRPC]
    public void PerfumeDone()
    {
        Debug.Log("checkshake4");
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
            Debug.Log("checkshake5");
        }
        else
        {
            photonView.RPC("FusionFail", RpcTarget.All);
        }

        perfumeNoteInfoLists.Clear();
        shakeRoutine = null;
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

    [PunRPC]
    public void ReceivePotion(float[] potionColor, float[] linePotionColor, PerfumeNoteName perfumeNoteName, PerfumeNoteState state)
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.02f;
            receiveCount++;

            Color colorPotion = new Color(potionColor[0], potionColor[1], potionColor[2], potionColor[3]);
            Color colorLinePotion = new Color(linePotionColor[0], linePotionColor[1], linePotionColor[2], linePotionColor[3]);

            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            m_MaterialPropertyBlock.SetColor("Color_E3091B1A", colorPotion);
            m_MaterialPropertyBlock.SetColor("Color_FDA61C50", colorLinePotion);

            liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

            if (receiveCount == 7)
            {
                foreach (var potionInfo in perfumeNoteInfoLists)
                {
                    if (potionInfo.Name == perfumeNoteName)
                    {
                        potionInfo.NoteCount += 1;
                        Debug.Log("추가됨");
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
        Debug.Log("리셋");
        yield return new WaitForSeconds(0.5f);
        fillAmount = Mathf.Round(fillAmount * 10f) / 10f;
        receiveCount = 0;
    }

    [PunRPC]
    void UpdateDropLiquid()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0 && perfumeClear)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            photonView.RPC("DecreaseLiquid", RpcTarget.AllBuffered, Time.deltaTime); 
        }
        else
        {
            particleSystemLiquid.Stop();
        }
    }

    [PunRPC]
    void DecreaseLiquid(float deltaTime)
    {
        var delta = 0.1f * deltaTime;
        fillAmount -= delta;

        if (fillAmount <= 0)
        {
            fillAmount = 0;
            photonView.RPC("ResetBottle", RpcTarget.All);
        }

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }


    [PunRPC]
    public void ResetBottle()
    {
        perfumeClear = false;
        resInfo = null;
        fillAmount = 0f; 
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        liquidMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

}

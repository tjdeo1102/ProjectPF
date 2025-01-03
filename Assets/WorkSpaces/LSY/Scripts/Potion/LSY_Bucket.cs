using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_Bucket : MonoBehaviourPun, IPunObservable
{
    [Header("액체 붓는 파티클")]
    public ParticleSystem particleSystemLiquid;

    [Header("최대 양동이 양")]
    public float targetFillAmount = 1f;

    [Header("양동이 양")]
    public float fillAmount;

    [Header("양동이 액체 렌더러 & 게임오브젝트")]
    public MeshRenderer MeshRenderer;
    public GameObject fillGameObject;

    [Header("현재 양동이가 가진 PerfumeNote")]
    public KSD_PerfumeNoteInfo currentPerfumeNote;

    [Header("양동이 액체 종류")]
    public List<LSY_BucketInfo> bucketInfos = new ();

    MaterialPropertyBlock m_MaterialPropertyBlock;
    Rigidbody m_RbPotion;

    private bool isFilling = false;

    Color potionColor;
    Color linePotionColor;

    void OnEnable()
    {
        particleSystemLiquid.Stop();
        
        fillGameObject.gameObject.SetActive(false);

        m_MaterialPropertyBlock = new MaterialPropertyBlock();

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        m_RbPotion = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 양동이가 기울어져 있고 & 액체가 들어있어야 하고 & 현재 노트가 Null이 아니여야 함
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0 && currentPerfumeNote.Name != PerfumeNoteName.Null)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            fillAmount -= 0.3f * Time.deltaTime;

            // 액체가 쏟아지는 방향으로 레이캐스트를 쏴서 LSY_DispensorReceiver컴포넌트를 가진 두 개의 충돌체가 있어야 액체를 받게 함
            RaycastHit[] hits = Physics.RaycastAll(particleSystemLiquid.transform.position, Vector3.down, 50.0f, ~0, QueryTriggerInteraction.Collide);

            int receiverCount = 0;
            LSY_DispensorLiquid[] receivers = new LSY_DispensorLiquid[hits.Length];

            foreach (RaycastHit hit in hits)
            {
                LSY_DispensorLiquid receiver = hit.collider.GetComponentInChildren<LSY_DispensorLiquid>();

                if (receiver != null)
                {
                    receivers[receiverCount] = receiver;
                    receiverCount++;

                    // 디스펜서의 노트와 양동이의 노트가 같지 않다면 디스펜서는 액체를 받을 수 없음
                    if (receiver.dispensorInfo.noteName != currentPerfumeNote.Name)
                    {
                        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
                        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
                        return;
                    }
                }
            }

            if (receiverCount >= 2)
            {
                Debug.Log("두 개의 DispensorReceiver를 찾음");
                LSY_DispensorLiquid receiver = receivers[0];
                receiver.photonView.RPC("ReceiveLiquid", RpcTarget.AllViaServer);
            }

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

        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cauldron") && !isFilling)
        {
            var cauldronController = collision.gameObject.GetComponent<KSD_CauldronController>();
            if (cauldronController == null) return;

            if (cauldronController.ResultNoteInfo.Name == PerfumeNoteName.Null) return;

            string cauldronName = collision.gameObject.name;  
            string resultNoteName = cauldronController.ResultNoteInfo.Name.ToString();  

            photonView.RPC("ChangeColor", RpcTarget.All, cauldronName, resultNoteName);
        }
    }


    [PunRPC]
    public void ChangeColor(string cauldronName, string resultNoteName)
    {
        if (currentPerfumeNote.Name != (PerfumeNoteName)Enum.Parse(typeof(PerfumeNoteName), resultNoteName))
        {
            fillAmount = 0;
        }

        currentPerfumeNote.Name = (PerfumeNoteName)Enum.Parse(typeof(PerfumeNoteName), resultNoteName);
        currentPerfumeNote.State = PerfumeNoteState.Note;

        foreach (var noteName in bucketInfos)
        {
            if (currentPerfumeNote.Name == noteName.noteName)
            {
                particleSystemLiquid = noteName.bucketParticle;
                m_MaterialPropertyBlock.SetColor("Color_E3091B1A", noteName.liquidColor);
                m_MaterialPropertyBlock.SetColor("Color_FDA61C50", noteName.liquidColor);
                MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            }
        }

        fillGameObject.gameObject.SetActive(true);

        // 양동이를 채우는 코루틴 시작
        if (fillBucketRoutine == null)
        {
            fillBucketRoutine = StartCoroutine(FillBucket());
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cauldron"))
        {
            if (fillBucketRoutine != null)
            {
                StopCoroutine(fillBucketRoutine);
                isFilling = false;
                fillBucketRoutine = null;
            }
        }
    }

    Coroutine fillBucketRoutine;

    // 양동이를 채워주는 코루틴, 3초동안 가져다대고 있으면 맥스로 채워짐
    private IEnumerator FillBucket()
    {
        isFilling = true;  
        float startTime = Time.time;  

        float initialFillAmount = fillAmount;  

        while (Time.time - startTime < 2f)
        {
            fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, (Time.time - startTime) / 2f);
            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
            yield return null;  
        }

        fillAmount = targetFillAmount;
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        MeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
        isFilling = false;
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

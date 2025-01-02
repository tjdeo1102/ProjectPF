using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_PotionReceiver : MonoBehaviourPun
{
    [Header("최대 포션 양")]
    public float maxLiquidFill = 1.0f;

    [Header("포션 양")]
    public float fillAmount = 0.0f;

    [Header("액체 렌더러")]
    public MeshRenderer liquidMeshRenderer;

    public List<KSD_PerfumeNoteInfo> perfumeNoteInfoLists;

    public int receiveCount = 0;


    private MaterialPropertyBlock m_MaterialPropertyBlock;

    void Start()
    {
        perfumeNoteInfoLists = new List<KSD_PerfumeNoteInfo> ();

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

    public void ReceiveCountReset()
    {
        fillAmount = Mathf.Round(fillAmount * 10f) / 10f;
        receiveCount = 0;
    }

    public void ReceivePotion(Color potionColor, Color linePotionColor, PerfumeNoteName perfumeNoteName)
    {
        if (fillAmount < maxLiquidFill)
        {
            fillAmount += 0.01f;
            receiveCount++;

            m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
            m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
            m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);

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
                noteInfo.NoteCount++;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fillAmount);
            stream.SendNext(perfumeNoteInfoLists);
        }
        else
        {
            fillAmount = (float)stream.ReceiveNext();
            perfumeNoteInfoLists = (List<KSD_PerfumeNoteInfo>)stream.ReceiveNext();
        }
    }
}

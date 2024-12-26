using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_ObjectFusion : MonoBehaviour
{
    [SerializeField] private List<KSD_PerfumeMaterialInfo> perfumeMatInfos;
    [SerializeField] private List<KSD_PerfumeNoteInfo> perfumeConcentrateInfos;
    [SerializeField] private List<KSD_PerfumeNoteInfo> perfumeNoteInfos;

    [Header("스폰 설정")]
    [SerializeField] private Transform spawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<KSD_MaterialObject>(out var mat))
        {
            perfumeMatInfos.Add(mat.data);
            PhotonNetwork.Destroy(mat.GetComponent<PhotonView>());
        }
        else if (other.TryGetComponent<KSD_LiquidObject>(out var liq))
        {
            if (liq.data.State == PerfumeNoteState.Concentrate)
            {
                perfumeConcentrateInfos.Add(liq.data);
            }
            else if (liq.data.State != PerfumeNoteState.Concentrate)
            {
                perfumeNoteInfos.Add(liq.data);
            }
            PhotonNetwork.Destroy(liq.GetComponent<PhotonView>());
        }
    }

    public void MakeConcentrate()
    {
        if (KSD_PerfumeManager.Instance.IsValidConcentrateRecipe(perfumeMatInfos, out var res))
        {
            Debug.Log($"원료 조합 성공! \n 원료 정보 {res}");

            var obj = PhotonNetwork.Instantiate("KSD_Concentrate", spawnPosition.position, Quaternion.identity);
            obj.GetComponent<KSD_LiquidObject>().SetInfo(res);
        }
    }

    public void MakeNote()
    {
        if (KSD_PerfumeManager.Instance.IsValidNoteRecipe(perfumeConcentrateInfos, out var res))
        {
            Debug.Log($"노트 조합 성공! \n 노트 정보 {res}");

            var obj = PhotonNetwork.Instantiate("KSD_Note", spawnPosition.position, Quaternion.identity);
            obj.GetComponent<KSD_LiquidObject>().SetInfo(res);
        }
    }

    public void MakePerfume()
    {
        if (KSD_PerfumeManager.Instance.IsValidPerfumeRecipe(perfumeNoteInfos, out var res))
        {
            Debug.Log($"향수 조합 성공 여부! \n 향수 정보 {res}");
        }
    }
}

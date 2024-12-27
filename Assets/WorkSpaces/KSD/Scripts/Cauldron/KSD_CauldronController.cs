using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class KSD_CauldronController : MonoBehaviourPun
{
    [Header("기본 설정")]
    public bool IsFail;         // 실패 여부
    public bool IsFinish;       // 완성 여부
    public List<KSD_PerfumeNoteInfo> ConcentrateInfos;
    public KSD_PerfumeNoteInfo ResultNoteInfo;

    [Header("상호작용 설정")]
    [SerializeField] private float finishCookTime;
    [SerializeField] private float noShakeDeadTime;
    [SerializeField] private float distancePerFrame;
    public bool IsActiveShake;
    public float CurrentPercentage;

    [Header("치트 모드")]
    public bool AlwaysShake;
    public bool AlwaysFire;

    [Header("참조 설정")]
    [SerializeField] private WGH_FanTest fire;

    private float cookTime;
    private float noShakeTime;
    private Vector3 lastSpoonPosition;

    // Update is called once per frame
    void Update()
    {
        // 이미 가공이 끝난 경우는 리턴
        if (fire == null || IsFinish) return;

        // 치트모드 설정 적용
        fire.AlwaysFire = AlwaysFire;
        if (AlwaysShake) IsActiveShake = AlwaysShake;

        // 불을 켜져 있지 않거나 넣은 재료가 없는 경우는 리턴
        if (fire.isActiveFire == false || ConcentrateInfos.Count < 1) return;

        // 1. 불을 켜져 있는 데, 섞고 있는 경우면 조리 시간 증가
        if (IsActiveShake)
        {
            cookTime += Time.deltaTime;
            if (cookTime >= finishCookTime) cookTime = finishCookTime;

            CurrentPercentage = cookTime / finishCookTime;
            if (CurrentPercentage >= 0.9f) NoteFusion();

            noShakeTime = 0f;   // 패널티 시간은 초기화
        }
        // 2. 불이 켜져 있는 데, 섞고 있지 않으면 패널티 시간 증가
        else
        {
            noShakeTime += Time.deltaTime;
            if (noShakeTime >= noShakeDeadTime)
            {
                IsFinish = true;
                IsFail = true;
            }
        }

        
    }
    [PunRPC]
    public void NoteFusionRPC()
    {
        var manager = KSD_PerfumeManager.Instance;
        // 3. 모든 조리 과정이 끝난 경우에는 갖고 있는 재료 조합
        if (manager != null)
        {
            IsFinish = true;
            // 성공한 경우에 해당 정보를 가진 결과물을 갖고 있어야함.
            IsFail = !manager.IsValidNoteRecipe(ConcentrateInfos, out ResultNoteInfo);
        }
    }
    private void NoteFusion()
    {
        photonView.RPC("NoteFusionRPC",RpcTarget.All);
    }

    public void ResetState()
    {
        // 이전 완료 상태 및 결과 노트 정보 초기화
        ConcentrateInfos.Clear();
        ResultNoteInfo = null;
        IsFinish = false;
        IsFail = false;
        CurrentPercentage = 0;

        // 이전 시간 상태 초기화
        noShakeTime = 0f;
        cookTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Concentrate")
            && other.TryGetComponent<KSD_LiquidObject>(out var con))
        {
            ConcentrateInfos.Add(con.data);
            PhotonNetwork.Destroy(con.gameObject);

            if (ConcentrateInfos.Count > 2)
            {
                IsFail = true;
                IsFinish = true;
            }
        }
        else if (other.CompareTag("Bucket")
            && other.TryGetComponent<KSD_LiquidObject>(out var note))
        {
            print("양동이에 담음");
            note.data = ResultNoteInfo;
            ResetState();
        }
        else if(other.CompareTag("Erase"))
        {
            //print("가마솥 리셋");
            ResetState();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Spoon"))
        {
            //print("충돌");
            var pos = other.gameObject.transform.position;
            IsActiveShake = Vector3.Distance(lastSpoonPosition, pos) > distancePerFrame;
            lastSpoonPosition = other.gameObject.transform.position;
        }
    }
}

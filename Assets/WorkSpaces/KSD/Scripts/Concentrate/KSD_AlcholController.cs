using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class KSD_AlcholController : MonoBehaviourPun
{
    [Header("기본 설정")]
    [SerializeField] private float outputLiquidTimer;           //액체가 나오는 시간

    [Header("참조 설정")]
    [SerializeField] private ParticleSystem outputParticle;     //액체가 나오는 효과를 담당하는 파티클

    private bool isPlaying;
    private float outputTimer;

    private KSD_PerfumeNoteInfo alcholInfo;

    private void Awake()
    {
        alcholInfo = new KSD_PerfumeNoteInfo() { Name = PerfumeNoteName.Alcohol, NoteCount = 1, State = PerfumeNoteState.Concentrate};
    }

    public void TakeAlchol()
    {
        photonView.RPC("TakeAlcholRPC", RpcTarget.All);
    }

    [PunRPC]
    public void TakeAlcholRPC()
    {
        // 액체가 나오지 않은 상태에서만 수행
        if (isPlaying == false)
        {
            isPlaying = true;
            outputTimer = outputLiquidTimer;

            if (outputParticle != null) outputParticle.Play();
        }
    }

    private void Update()
    {
        // 액체가 나오기 시작하면 작동되도록
        if (isPlaying == false) return;

        if (outputTimer < 0)
        {
            isPlaying = false;
            if (outputParticle != null) outputParticle.Stop();
        }

        Debug.DrawRay(outputParticle.transform.position, Vector3.down, Color.red);

        // 자기자신 임시로 레이캐스트 제외되는 레이어로 바꾸기
        int tempLayer = gameObject.layer;
        gameObject.layer = 8;
        if (Physics.Raycast(outputParticle.transform.position, Vector3.down, out var hit, 50.0f, ~(1 << 8), QueryTriggerInteraction.Collide))
        {
            if (hit.collider.TryGetComponent<KSD_ConcentrateBottle>(out var receiver))
            {
                receiver.ReceiveConcentrate(alcholInfo, Time.deltaTime / outputLiquidTimer);
                Debug.Log("받을 KSD_ConcentrateBottle를 찾음");
            }
        }
        gameObject.layer = tempLayer;

        // 액체가 나오는 타이머 카운트 (해당 타이머가 끝나면 액체가 안나옴)
        outputTimer -= Time.deltaTime;
    }
}

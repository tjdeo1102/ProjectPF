using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_MixerReceiver : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float outputLiquidTimer;           //액체가 나오는 시간

    [Header("참조 설정")]
    [SerializeField] private ParticleSystem outputParticle;     //액체가 나오는 효과를 담당하는 파티클
    [SerializeField] private PTK_Box mixer;                     //믹서기 완료를 알기위해 이벤트 구독

    private KSD_ConcentrateBottle currentBottle;                //갈린 액체를 받을 병
    private KSD_PerfumeMaterialInfo resultMixInfo;              //믹서기 결과로 나온 액체
    private bool isMixDone;
    private float outputTimer;
    private int overlapBottleCount = 0;

    private void OnEnable()
    {
        if (mixer != null)
        {
            mixer.OnMixDone += MixDone;
        }
    }

    private void OnDisable()
    {
        if (mixer != null)
        {
            mixer.OnMixDone -= MixDone;
        }
    }

    //1. 믹서기가 다 갈리면 액체가 나오기 시작 (RPC로 해당 함수 호출)
    public void MixDone(KSD_PerfumeMaterialInfo newInfo)
    {
        // 액체가 나오지 않은 상태에서만 수행
        if (isMixDone == false)
        {
            isMixDone = true;
            resultMixInfo = new KSD_PerfumeMaterialInfo();
            resultMixInfo.Name = newInfo.Name;
            resultMixInfo.Type = newInfo.Type;
            resultMixInfo.State = newInfo.State;
            outputTimer = outputLiquidTimer;

            if (outputParticle != null) outputParticle.Play();
        }
    }

    //2-1. 액체가 나오는 동안, 지속적인 바닥 콜리전과 포션병과 물리 충돌 확인을 통해, 병 상태 업데이트
    private void OnCollisionEnter(Collision collision)
    {
        var trans = collision.transform;
        
        // 받고 있는 도중에 새로운 병이 추가되도, 기존병이 없어지지 않으면 반응 없음
        if (currentBottle == null)
        {
            overlapBottleCount++;
            trans.TryGetComponent<KSD_ConcentrateBottle>(out currentBottle);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var trans = collision.transform;
        if (trans.TryGetComponent<KSD_ConcentrateBottle>(out currentBottle) && overlapBottleCount <= 1)
        {
            overlapBottleCount--;
            currentBottle = null;
        }
    }

    //2-2. 액체의 FillAmount가 임계치를 넘어서면 1로 다 채워지도록 수정
    private void Update()
    {
        // 액체가 나오기 시작하면 작동되도록
        if (isMixDone == false) return;

        if (outputTimer < 0)
        {
            isMixDone = false;
            if (outputParticle != null) outputParticle.Stop();
        }

        if (currentBottle != null)
        {
            //3. 받았던 완성된 재료액의 Info의 정보를 업데이트
            currentBottle.ReceiveLiquidMaterial(resultMixInfo, Time.deltaTime / outputLiquidTimer);
        }

        // 액체가 나오는 타이머 카운트 (해당 타이머가 끝나면 액체가 안나옴)
        outputTimer -= Time.deltaTime;
    }

}

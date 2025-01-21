using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static KSH_AudioManager;

[RequireComponent(typeof(PhotonView))]
public class KSD_CauldronController : MonoBehaviourPun
{
    [Header("기본 설정")]
    public bool IsFinish;       // 완성 여부
    public List<KSD_PerfumeNoteInfo> ConcentrateInfoList;
    public List<float> ConcentrateAmountList;
    public KSD_PerfumeNoteInfo ResultNoteInfo;
    [Range(0f,1f),SerializeField] private float maxFillConcentrateThreshold;     //1회분 원료로 판단할 임계치

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
    [SerializeField] private ParticleSystem fireSmoke;
    [SerializeField] private ParticleSystem successMake;
    [SerializeField] private ParticleSystem successSmell;
    [SerializeField] private ParticleSystem failMake;
    [SerializeField] private KSD_CauldronUI ui;

    private float cookTime;
    private float noShakeTime;
    private Vector3 lastSpoonPosition;

    // Update is called once per frame
    void Update()
    {
        // 화력이 충분할 때, 연기 이펙트 활성화
        if (fire.wasActiveFire || AlwaysFire)
        {
            fireSmoke.Play();
            photonView.RPC("RPC_PlayCauldronSfx", RpcTarget.All, 24);
        }
        else
        {
            fireSmoke.Stop();
            photonView.RPC("RPC_StopCauldronInputSfx", RpcTarget.All, 24);
        }

        // 이미 가공이 끝난 경우는 리턴
        if (fire == null || IsFinish) return;

        // 치트모드 설정 적용
        fire.AlwaysFire = AlwaysFire;
        if (AlwaysShake) IsActiveShake = AlwaysShake;

        // 불을 켜져 있지 않거나 넣은 재료가 없는 경우는 리턴
        if (fire.isActiveFire == false || ConcentrateInfoList.Count < 1) return;

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
            if (manager.IsValidNoteRecipe(ConcentrateInfoList, out ResultNoteInfo))
            {
                var a = successSmell.main.startColor.color.a;
                var col = ResultNoteInfo.GetColorByName(ResultNoteInfo.Name);
                var mainSmell = successSmell.main;
                var mainMake = successMake.main;
                mainSmell.startColor = new ParticleSystem.MinMaxGradient(new Color(col.r, col.g, col.b, a));
                mainMake.startColor = col;
                successSmell.Play();
                successMake.Play();
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Cauldron_success);
            }
            else
            {
                failMake.Play();
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Cauldron_fall);
            }
            
            ConcentrateInfoList.Clear();
            ConcentrateAmountList.Clear();
        }
    }
    private void NoteFusion()
    {
        photonView.RPC("NoteFusionRPC",RpcTarget.All);
    }

    public void ResetState()
    {
        // 이전 완료 상태 및 결과 노트 정보 초기화
        ConcentrateInfoList.Clear();
        ConcentrateAmountList.Clear();
        ResultNoteInfo = new KSD_PerfumeNoteInfo { Name = PerfumeNoteName.Null, NoteCount = 0, State = PerfumeNoteState.Null};
        IsFinish = false;
        CurrentPercentage = 0;

        // 이전 시간 상태 초기화
        noShakeTime = 0f;
        cookTime = 0f;
        successSmell.Stop();
    }

    public void ReceiveConcentrate(KSD_PerfumeNoteInfo con, float getAmount)
    {
        // 0. 기존에 노트가 있는 경우에는 재료 추가 안됨
        if (ResultNoteInfo.Name != PerfumeNoteName.Null) return;
        // 0-1. 원료의 상태가 잘못되었으면 추가 안됨
        if (con.State != PerfumeNoteState.Concentrate) return;

        var newCon = new KSD_PerfumeNoteInfo { Name = con.Name, State = con.State, NoteCount = 0};

        // 1. 재료리스트 중에 일치하는 이름이 없으면 새로 추가
        // 일치하는 아이템이 있는지 확인
        int index = ConcentrateInfoList.FindIndex(item => item.Name == newCon.Name);

        // 1-1. 재료 리스트의 카운트가 2이상인데 새로운 재료가 추가되면
        // 꽉차있다고 가정하고 새로운 재료 추가 안되고 리턴
        if (index == -1)
        {
            if (ConcentrateInfoList.Count >= 2) return;
            // 새로운 재료 추가
            else
            {
                index = ConcentrateInfoList.Count;
                ConcentrateInfoList.Add(newCon);
                ConcentrateAmountList.Add(0);

                photonView.RPC("RPC_PlayCauldronSfx", RpcTarget.All, 21);
            }
        }

        // 들어온 재료 양 더해주기
        ConcentrateAmountList[index] += getAmount;

        // 재료가 임계점을 넘은 경우에는 다 찼다고 판정
        if (ConcentrateAmountList[index] > maxFillConcentrateThreshold)
        {
            ConcentrateAmountList[index] = 0f;
            ConcentrateInfoList[index].NoteCount += 1;
            if (ui != null)
            {
                ui.SetText($"원료 {ConcentrateInfoList[index].Name.ToString()}가 1회분 추가됨", newCon.GetColorByName(newCon.Name));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Erase"))
        {
            if (other.gameObject.TryGetComponent<XRBaseInteractable>(out var a) && a.isSelected == false)
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
            //print("가마솥 리셋");
            //if (PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(other.gameObject);
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

    [PunRPC]
    protected void RPC_PlayCauldronSfx(int sfx)
    {
        KSH_AudioManager.Instance.PlaySfx((KSH_AudioManager.Sfx)sfx);
    }

    [PunRPC]
    protected void RPC_StopCauldronInputSfx(int sfx)
    {
        KSH_AudioManager.Instance.StopInputSfx((KSH_AudioManager.Sfx)sfx);
    }
}

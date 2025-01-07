using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(PhotonView),typeof(KSD_NetworkGrabInteractable))]
public class KSD_ConcentrateBottle : MonoBehaviourPun
{
    static private int NextFreeUniqueId = 3000;

    [Header("기본 설정")]
    [Range(0f, 1f), SerializeField] private float maxFillMateiralThreshold;    // 각 재료가 최대치로 바뀌는 임계치
    [Range(0f, 1f), SerializeField] private float maxFillNoteThreshold;        // 노트가 최대치로 바뀌는 임계치
    [SerializeField] private float shakeTimer;                                 // 얼마나 흔들었을 때, 조합될지 타이머
    [SerializeField] private float distancePerFrame;                           // 프레임당 얼마나 거리차이가 날때 상태체크할지 결정
    [SerializeField] private float startFillAmount;
    public float fillAmount;

    [Header("조합 전 재료 리스트")]
    public List<KSD_PerfumeMaterialInfo> perfumeMaterialList;                // 조합전 재료 리스트

    [Header("조합 후 원료 정보")]
    public KSD_PerfumeNoteInfo resInfo;

    [Header("참조 설정")]
    [SerializeField] private ParticleSystem particleSystemLiquid;
    [SerializeField] private ParticleSystem particleSystemSplash;
    public MeshRenderer LiquidRenderer;

    private float lastShakeTime;       
    private float deadTime = 1f;       // 흔들림 중단으로 간주할 시간
    private bool IsActiveShake;
    private Vector3 lastSpoonPosition;
    private bool isGrab;

    private Coroutine shakeRoutine;

    MaterialPropertyBlock m_MaterialPropertyBlock;
    Rigidbody m_RbPotion;

    int m_UniqueId;
    bool m_Breakable;

    public Color potionColor;
    public Color linePotionColor;

    private void OnEnable()
    {
        particleSystemLiquid.Stop();
        if (particleSystemSplash)
            particleSystemSplash.Stop();

        m_MaterialPropertyBlock = new MaterialPropertyBlock();
        LiquidRenderer.SetPropertyBlock(m_MaterialPropertyBlock);

        fillAmount = startFillAmount;

        m_RbPotion = GetComponent<Rigidbody>();
        m_Breakable = false;

        if (fillAmount < 0.001f) resInfo = new KSD_PerfumeNoteInfo()
        {
            Name = PerfumeNoteName.Null,
            State = PerfumeNoteState.Null,
            NoteCount = 0
        };

        perfumeMaterialList = new List<KSD_PerfumeMaterialInfo>();

        var grab = GetComponent<KSD_NetworkGrabInteractable>();
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        var grab = GetComponent<KSD_NetworkGrabInteractable>();
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    void Start()
    {
        m_UniqueId = NextFreeUniqueId++;
    }

    // Update is called once per frame
    void Update()
    {
        // 조합 전에 재료만 채워져 있는 경우에는 흰색으로 초기화
        if (resInfo.Name == PerfumeNoteName.Null
            && resInfo.State == PerfumeNoteState.Null)
        {
            potionColor = resInfo.GetColorByName(PerfumeNoteName.Null);
            linePotionColor = resInfo.GetColorByName(PerfumeNoteName.Null);

            CheckShake();
        }
        
        UpdateDropLiquid();

        // 변수에 따라 항상 업데이트
        LiquidRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
        m_MaterialPropertyBlock.SetFloat("LiquidFill", fillAmount);
        m_MaterialPropertyBlock.SetColor("Color_E3091B1A", potionColor);
        m_MaterialPropertyBlock.SetColor("Color_FDA61C50", linePotionColor);
        LiquidRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

    private void ResetBottle()
    {
        resInfo = new KSD_PerfumeNoteInfo();
        resInfo.Name = PerfumeNoteName.Null;
        resInfo.State = PerfumeNoteState.Null;

        if (perfumeMaterialList.Count > 0) perfumeMaterialList.Clear();
    }

    private void CheckShake()
    {
        // 액체를 흔들고 있는 경우 상태 체크
        var pos = transform.position;
        bool isShakingNow = Vector3.Distance(lastSpoonPosition, pos) > distancePerFrame;

        if (isShakingNow && isGrab)
        {
            lastShakeTime = Time.time; // 마지막 Shake 시간 갱신
            IsActiveShake = true;

            // 흔들림 루틴 시작
            if (shakeRoutine == null)
                shakeRoutine = StartCoroutine(ShakeRoutine());
        }
        else
        {
            // deadTime이 초과되는 만큼 Shake를 멈추면 루틴 해제
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

        lastSpoonPosition = pos; // 현재 위치 저장
    }

    private IEnumerator ShakeRoutine()
    {
        yield return new WaitForSeconds(shakeTimer);
        shakeRoutine = null;
        photonView.RPC("FusionRPC", RpcTarget.All);
    }

    [PunRPC]
    public void FusionRPC()
    {
        if (KSD_PerfumeManager.Instance == null) return;
        // 이미 다른 RPC에 의해 조합이 수행된 경우에는 리턴 (싱글 스레드이므로 함수 전체 실행 보장)
        if (perfumeMaterialList.Count < 1) return;
        // 처음 호출되는 RPC인 경우에는 조합 진행 

        if (KSD_PerfumeManager.Instance.IsValidConcentrateRecipe(perfumeMaterialList, out var res))
        {
            // 성공했을 경우에는 해당 res정보로 업데이트
            resInfo = new KSD_PerfumeNoteInfo();
            resInfo.Name = res.Name;
            resInfo.State = res.State;
            resInfo.NoteCount = res.NoteCount;

            potionColor = resInfo.GetColorByName(res.Name);
            linePotionColor = resInfo.GetColorByName(res.Name);
        }
        else FusionFail();
        perfumeMaterialList.Clear();
    }

    // 조합했는데 실패했을 경우 호출되는 함수
    private void FusionFail()
    {
        // 이름은 Null, 상태는 Note로 초기화 된다.
        resInfo = new KSD_PerfumeNoteInfo();
        resInfo.Name = PerfumeNoteName.Null;
        resInfo.State = PerfumeNoteState.Note;

        potionColor = Color.black;
        linePotionColor = Color.black;
    }

    void UpdateDropLiquid()
    {
        if (Vector3.Dot(transform.up, Vector3.down) > 0 && fillAmount > 0)
        {
            if (particleSystemLiquid.isStopped)
            {
                particleSystemLiquid.Play();
            }

            var delta = 0.1f * Time.deltaTime;
            fillAmount -= delta;
            // 모든 액체를 버린 경우에는 정보가 초기화
            if (fillAmount < 0.01f)
            {
                fillAmount = 0;
                ResetBottle();
            }
            // 자기자신 임시로 레이캐스트 제외되는 레이어로 바꾸기
            int tempLayer = gameObject.layer;
            gameObject.layer = 8;

            Debug.DrawRay(particleSystemLiquid.transform.position, Vector3.down, Color.red);
            if (Physics.Raycast(particleSystemLiquid.transform.position, Vector3.down, out var hit, 50.0f, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.TryGetComponent<KSD_ConcentrateBottle>(out var receiver))
                {
                    receiver.ReceiveConcentrate(resInfo, delta);
                    Debug.Log("받을 KSD_ConcentrateBottle를 찾음");
                }
                else if(hit.transform.CompareTag("Cauldron"))
                {
                    var receiver2 = hit.collider.GetComponentInParent<KSD_CauldronController>();
                    if (receiver2 != null)
                    {
                        receiver2.ReceiveConcentrate(resInfo, delta);
                        Debug.Log("받을 KSD_CauldronController를 찾음");
                    }
                    else Debug.Log("PotionReceiver를 찾지 못함");
                }
                else
                {
                    Debug.Log("PotionReceiver를 찾지 못함");
                }
            }

            gameObject.layer = tempLayer;
        }
        else particleSystemLiquid.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ingredient"))
        {
            // 추후, 가루 오브젝트의 컴포넌트에 따라 변동 가능
            if (collision.transform.TryGetComponent<PTK_Fruit>(out var res))
            {
                // 가루화된 약초인 경우에만 동작
                if (res.fruitInfo.Type == PerfumeMaterialType.Hub && res.fruitInfo.State == PerfumeMaterialState.Process)
                {
                    ReceiveLiquidMaterial(res.fruitInfo, 1f);
                    if (PhotonNetwork.IsMasterClient) PhotonNetwork.Destroy(collision.gameObject);
                }
            }
        }

        if (m_Breakable && m_RbPotion.velocity.magnitude > 1.35)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            if (particleSystemSplash != null)
            {
                particleSystemSplash.gameObject.SetActive(true);
                if (fillAmount > 0)
                {
                    particleSystemSplash.Play();
                }
            }
            Destroy(this);
        }
    }

    public void ReceiveLiquidMaterial(KSD_PerfumeMaterialInfo mat, float getAmount)
    {
        // 0. 기존에 원료가 있는 경우에는 재료 추가 안됨
        if (resInfo.Name != PerfumeNoteName.Null) return;
        // 0-1. mat의 상태가 잘못되었으면 추가 안됨
        if (mat.State != PerfumeMaterialState.Process) return;

        var newMat = new KSD_PerfumeMaterialInfo { Name = mat.Name, State = mat.State, Type = mat.Type, MaterialAmount = 0f };

        // 1. 재료리스트 중에 일치하는 이름이 없으면 새로 추가
        // 일치하는 아이템이 있는지 확인
        var findItem = perfumeMaterialList.FirstOrDefault(item => item.Name == newMat.Name);

        // 1-1. 재료 리스트의 카운트가 2이상인데 새로운 재료가 추가되면
        // 꽉차있다고 가정하고 새로운 재료 추가 안되고 리턴
        if (findItem == null)
        {
            if (perfumeMaterialList.Count >= 2) return;
            // 새로운 재료 추가
            else perfumeMaterialList.Add(newMat);
        }
        // 리스트에 있는 경우에는 newmat 갱신
        else newMat = findItem;

        // 들어온 재료 양 더해주기
        newMat.MaterialAmount += getAmount;

        // 재료가 임계점을 넘은 경우에는 다 찼다고 판정
        if (newMat.MaterialAmount > maxFillMateiralThreshold) newMat.MaterialAmount = 1f;

        // 현재 재료의 Amount에 따라 fillAmount업데이트
        float newAmount = 0f;
        foreach (var item in perfumeMaterialList)
        {
            newAmount += item.MaterialAmount * 0.5f;
        }
        fillAmount = newAmount;
    }

    public void ReceiveConcentrate(KSD_PerfumeNoteInfo note, float getAmount)
    {
        // 만약, 현재 병에 이름이 다른 원료가 채워진 경우에는 리턴
        if (resInfo.Name != PerfumeNoteName.Null && resInfo.Name != note.Name) return;
        // 현재 병에 재료들이 채워져 있는 경우에도 리턴
        if (perfumeMaterialList.Count > 0) return;

        // 같은 재료인 경우거나 병이 비워진 경우에만 정보 업데이트
        resInfo.Name = note.Name;
        resInfo.State = note.State;
        fillAmount += getAmount;

        if (fillAmount > maxFillNoteThreshold)
        {
            fillAmount = 1f;
            resInfo.NoteCount = 1;
        }
    }


    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrab = true;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isGrab = false;
    }
}

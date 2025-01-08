using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Unity.VisualScripting;

public class WGH_SmellStick : MonoBehaviourPun
{
    [SerializeField] private GameObject customer;
    [SerializeField] private float interactionDist;     // 상호작용 거리
    [SerializeField] private ParticleSystem[] aura;

    private float curTime;                              // 현재 시간
    [SerializeField] private float needTime;            // 시향에 필요한 시간
    [SerializeField] private float returnDistance;      // 멀어졌을 때 원래위치로 돌아오는 거리
    public E_WGH_NoteType NoteType;
    [SerializeField] WGH_InteractionNote contactNote;
    public event Action OnBestInteract;
    public event Action OnLikeInteract;
    public event Action OnQuestionInteract;
    public event Action OnDespairInteract;
    
    private bool isAbsorbed;                            // 이펙트 On인지 아닌지(상호작용 가능한 상태인지)
    private bool isRoutine;
    [HideInInspector] public bool isGrab;
    private Rigidbody rigid;
    private Vector3 startPos;
    private Coroutine timeRoutine;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPos = transform.position;
    }
    private void LateUpdate()
    {
        //// "Dynamic Attach"라는 이름을 가진 자식 오브젝트 삭제
        //Transform dynamicAttach = transform.Find("[Ray Interactor] Dynamic Attach");
        //if (dynamicAttach != null)
        //{
        //    Destroy(dynamicAttach.gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_InteractionNote note))
        {
            contactNote = note;
        }
        if (other.gameObject.TryGetComponent(out WGH_InteractArea interactArea) && isAbsorbed == true && isRoutine == false)
        {
            customer = interactArea.GetComponentInParent<WGH_NPCController>().gameObject;
            curTime = 0f;
            timeRoutine = StartCoroutine(TimeRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_InteractArea interactArea))
        {
            OnBestInteract = null;
            OnLikeInteract = null;
            OnQuestionInteract = null;
            OnDespairInteract = null;
            if(isRoutine)
            {
                StopCoroutine(timeRoutine);
                isRoutine = false;
            }
        }
    }

    IEnumerator TimeRoutine()
    {
        isRoutine = true;
        while (true)
        {
            curTime += Time.deltaTime;
            if(curTime >= needTime)
            {
                React();
                curTime = 0f;
                OffEffect();
                isRoutine = false;
                yield break;
            }
            yield return null;
        }
    }

    private void React()
    {
        Debug.Log("상호작용");
        if (NoteType == customer.GetComponent<WGH_NPCController>().BestMaterial)
        {
            OnBestInteract?.Invoke();
        }
        else if (NoteType == customer.GetComponent<WGH_NPCController>().LikeMaterial || NoteType == customer.GetComponent<WGH_NPCController>().LikeMaterial2)
        {
            OnLikeInteract?.Invoke();
        }
        else if (NoteType == customer.GetComponent<WGH_NPCController>().QuestionMaterial || NoteType == customer.GetComponent<WGH_NPCController>().QuestionMaterial2)
        {
            OnQuestionInteract?.Invoke();
        }
        else
        {
            OnDespairInteract?.Invoke();
        }
    }

    /// <summary>
    /// 시향노트에서 빠졌을 때 이펙트 On
    /// </summary>
    public void OnEffect()
    {
        photonView.RPC("EffectRPC", RpcTarget.All, true);
    }
    /// <summary>
    /// 시향노트에 끼워졌을 때 이펙트 Off
    /// </summary>
    public void OffEffect()
    {
        photonView.RPC("EffectRPC", RpcTarget.All, false);
    }

    [PunRPC]
    public void EffectRPC(bool enable)
    {
        aura[(int)NoteType - 1].gameObject.SetActive(enable);
        isAbsorbed = enable;
    }
}

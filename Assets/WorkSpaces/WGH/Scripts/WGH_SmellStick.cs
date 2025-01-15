using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;

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
    private bool isGrab;
    private Rigidbody rigid;
    private Vector3 startPos;
    private Coroutine shakeRoutine;

    private float judgeAmount;
    [SerializeField] private float judgeCurLate;
    private Vector3 lastPos;

    private void Awake()
    {
        judgeAmount = 3;
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPos = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(isGrab == false)
        transform.position = startPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_InteractionNote note))
        {
            contactNote = note;
            lastPos = transform.position;
        }
        else if (other.gameObject.TryGetComponent(out WGH_InteractArea interactArea) && isAbsorbed == true && isRoutine == false)
        {
            customer = interactArea.GetComponentInParent<WGH_NPCController>().gameObject;
            judgeCurLate = 0f;
            shakeRoutine = StartCoroutine(ShakeRoutine());
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Test2);
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
                StopCoroutine(shakeRoutine);
                isRoutine = false;
            }
        }
    }
    IEnumerator ShakeRoutine()
    {
        isRoutine = true;
        while (true)
        {
            float dist = Vector3.Distance(transform.position, lastPos);
            if (dist > 0.1f)
            {
                judgeCurLate += 0.1f;
                lastPos = transform.position;
            }
            if (judgeCurLate >= judgeAmount)
            {
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Guest_feedback);
                React();
                judgeCurLate = 0;
                OffEffect();
                isRoutine = false;
                yield break;
            }
            yield return null;
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

    /// <summary>
    /// 물체를 잡았을 때 호출
    /// </summary>
    public void OnGrab()
    {
        photonView.RPC("IsGrabRPC", RpcTarget.All, true);
    }

    /// <summary>
    /// 물체를 놓았을 때 호출
    /// </summary>
    public void OnRelease()
    {
        photonView.RPC("IsGrabRPC", RpcTarget.All, false);
    }


    /// <summary>
    /// 중력 상태 동기화
    /// </summary>
    [PunRPC]
    public void IsGrabRPC(bool isGrabbed)
    {
        isGrab = isGrabbed;
    }
}

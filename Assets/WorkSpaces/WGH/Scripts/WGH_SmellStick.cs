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
    [SerializeField] private ParticleSystem aura;
    public E_WGH_NoteType NoteType;
    public event Action OnBestInteract;
    public event Action OnLikeInteract;
    public event Action OnQuestionInteract;
    public event Action OnDespairInteract;

    private float curTime;                              // 현재 시간
    [SerializeField] private float needTime;            // 시향에 필요한 시간
    private bool isAbsorbed;
    private bool isRoutine;

    private Coroutine timeRoutine;
    public void Interact()
    {
        StartCoroutine(InteractRoutine());
    }
    /// <summary>
    /// 시향노트에서 빠졌을 때 이펙트 On
    /// </summary>
    public void OnEffect()
    {
        aura.gameObject.SetActive(true);
        isAbsorbed = true;
    }

    public void OffEffect()
    {
        aura.gameObject.SetActive(false);
        isAbsorbed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.TryGetComponent(out WGH_InteractionNote testNote) && NoteType == testNote.NoteType)
        //{
        //    Debug.Log("이펙트");
        //    aura.gameObject.SetActive(true);
        //    isAbsorbed = true;
        //}
        if (other.gameObject.TryGetComponent(out WGH_InteractArea interactArea) && isAbsorbed == true)
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
            customer = null;
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
        while (true)
        {
            isRoutine = true;
            Debug.Log(curTime);
            curTime += Time.deltaTime;
            if(curTime >= needTime)
            {
                StartCoroutine(InteractRoutine());
                curTime = 0f;
                aura.gameObject.SetActive(false);
                isAbsorbed = false;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator InteractRoutine()
    {
        yield return new WaitForSeconds(1);
        if (Vector3.Distance(transform.position, customer.transform.position) < interactionDist)
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
        yield break;
    }
}

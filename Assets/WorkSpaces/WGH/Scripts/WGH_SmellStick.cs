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
    [SerializeField] private float interactionDist;
    public E_WGH_NoteType NoteType;
    public event Action OnBestInteract;
    public event Action OnLikeInteract;
    public event Action OnQuestionInteract;
    public event Action OnDespairInteract;

    private float interactTime;
    [SerializeField] private float needTime;

    private Coroutine timeRoutine;
    public void Interact()
    {
        StartCoroutine(InteractRoutine());
    }
    
    IEnumerator InteractRoutine()
    {
        yield return new WaitForSeconds(1);
        if (Vector3.Distance(transform.position, customer.transform.position) < interactionDist)
        {
            Debug.Log("상호작용");
            if(NoteType == customer.GetComponent<WGH_NPCController>().BestMaterial)
            {
                OnBestInteract?.Invoke();
            }
            else if(NoteType == customer.GetComponent<WGH_NPCController>().LikeMaterial || NoteType == customer.GetComponent<WGH_NPCController>().LikeMaterial2)
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
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out WGH_InteractArea interactArea))
        {
            customer = interactArea.GetComponentInParent<WGH_NPCController>().gameObject;
            interactTime = 0f;
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
            StopCoroutine(timeRoutine);
        }
    }

    IEnumerator TimeRoutine()
    {
        while (true)
        {
            Debug.Log(interactTime);
            interactTime += Time.deltaTime;
            if(interactTime >= needTime)
            {
                StartCoroutine(InteractRoutine());
                interactTime = 0f;
                yield break;
            }
            yield return null;
        }
    }
}

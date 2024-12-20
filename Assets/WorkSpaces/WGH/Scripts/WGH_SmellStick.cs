using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WGH_SmellStick : MonoBehaviour
{
    [SerializeField] private GameObject customer;
    [SerializeField] private float interactionDist;
    public E_WGH_PerfumeMaterialType PerfumeMaterialType;
    public event Action OnBestInteract;
    public event Action OnLikeInteract;
    public event Action OnQuestionInteract;
    public event Action OnDespairInteract;
    
    public void Interact()
    {
        StartCoroutine(InteractRoutine());
    }
    
    IEnumerator InteractRoutine()
    {
        customer = GameObject.FindGameObjectWithTag("Customer");
        yield return new WaitForSeconds(1);
        if (Vector3.Distance(transform.position, customer.transform.position) < interactionDist)
        {
            Debug.Log("상호작용");
            if(PerfumeMaterialType == customer.GetComponent<WGH_NPCController>().BestMaterial)
            {
                OnBestInteract?.Invoke();
            }
            else if(PerfumeMaterialType == customer.GetComponent<WGH_NPCController>().LikeMaterial)
            {
                OnLikeInteract?.Invoke();
            }
            else if (PerfumeMaterialType == customer.GetComponent<WGH_NPCController>().QuestionMaterial)
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
        if (other.gameObject.TryGetComponent(out WGH_TestPerfumeMaterial perfume))
        {
            PerfumeMaterialType = perfume.PerfumeMaterialType;
        }
    }
}

using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(KSD_NetworkGrabInteractable))]
public class KSD_FruitSpoon : MonoBehaviour
{
    [SerializeField] private Transform attachTransform;
    private KSD_NetworkGrabInteractable grapInteractable;
    private Collider[] cols;
    private bool isActive;
    private PTK_Fruit grabObject;
    PhotonView photonView;

    private void Awake()
    {
        cols = GetComponentsInChildren<Collider>();
        grapInteractable = GetComponent<KSD_NetworkGrabInteractable>();
        grapInteractable.selectEntered.AddListener(OnSelectEntered);
        grapInteractable.selectExited.AddListener(OnSelectExited);
        grapInteractable.activated.AddListener(OnActivated);
        grapInteractable.deactivated.AddListener(OnDeactivated);
    }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnDisable()
    {
        grapInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grapInteractable.selectExited.RemoveListener(OnSelectExited);
        grapInteractable.activated.RemoveListener(OnActivated);
        grapInteractable.deactivated.RemoveListener(OnDeactivated);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].isTrigger = true;
        }
    }
    private void OnSelectExited(SelectExitEventArgs args)
    {
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].isTrigger = false;
        }
    }
    private void OnActivated(ActivateEventArgs arg)
    {
        isActive = true;
    }
    private void OnDeactivated(DeactivateEventArgs arg0)
    {
        isActive = false;
        grabObject = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive == false || grabObject != null) return;
        if (other.transform.TryGetComponent<PTK_Fruit>(out var fruit))
        {
            // 작은 과일들 스푼으로 가져갈 수 있도록 만들기
            if (fruit.fruitInfo.State == PerfumeMaterialState.Raw
                && fruit.fruitInfo.Type == PerfumeMaterialType.Small)
            {
                photonView.RPC("RPC_PlaySfx", RpcTarget.All, 6);
                grabObject = fruit;
            }
            // 허브 가루들도 가져갈 수 있도록 만들기
            else if (fruit.fruitInfo.State == PerfumeMaterialState.Process
                && fruit.fruitInfo.Type == PerfumeMaterialType.Hub)
            {
                photonView.RPC("RPC_PlaySfx", RpcTarget.All, 15);
                grabObject = fruit;
            }
        }
    }

    private void Update()
    {
        if (grabObject == null) return;
        grabObject.transform.position = attachTransform.position;
    }

    [PunRPC]
    private void RPC_PlaySfx(int sfx)
    {
        KSH_AudioManager.Instance.PlaySfx((KSH_AudioManager.Sfx)sfx);
    }
}

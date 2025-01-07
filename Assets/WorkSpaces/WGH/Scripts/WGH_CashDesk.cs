using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WGH_CashDesk : MonoBehaviour
{
    private XRSocketInteractor socket;

    private GameObject npcObj;
    public WGH_NPCController Customer;

    private GameObject perfume;

    private bool isReady;
    
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        npcObj = GameObject.FindGameObjectWithTag("TestNote");

        socket.selectEntered.AddListener(FindCustomer);
        socket.selectExited.AddListener(DeleteCustomer);
    }

    void Update()
    {
        if(isReady == true && Customer != null && perfume != null && PhotonNetwork.IsMasterClient == true)
        {
            isReady = false;
        }
    }

    public void FindCustomer(SelectEnterEventArgs arg)
    {
        // ¼Õ´Ô Å½»ö
        Customer = npcObj.transform.GetChild(0).GetComponent<WGH_InteractionNote>().Customer;
        // Çâ¼ö Å½»ö
        perfume = socket.gameObject;
    }

    public void DeleteCustomer(SelectExitEventArgs arg)
    {
        // ¼Õ´Ô null
        Customer = null;
    }
}

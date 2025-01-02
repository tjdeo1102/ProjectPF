using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;


public class WGH_InteractionNote : MonoBehaviourPun
{
    public E_WGH_NoteType NoteType;
    public WGH_SmellStick SmellStick;
    public XRSocketInteractor socket;
    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }
    private void Start()
    {
        SmellStick = GameObject.FindGameObjectWithTag("SmellStick").gameObject.GetComponent<WGH_SmellStick>();
        //socket.allowSelect = false;             // 자동 소켓 비활성화
        
    }

    public void EnableSocketInteraction(bool enable)
    {
        photonView.RPC("SocketInteractionRPC", RpcTarget.All, enable);
    }

    [PunRPC]
    public void SocketInteractionRPC(bool enable)       // 손을 놨을때가 false임.
    {
        if(enable == false)
        {
            StartCoroutine(SocketActiveRoutine());
            Debug.Log("손 뗌");
        }
        
        if(enable == true && socket.isSelectActive)
        {
            socket.EndManualInteraction();
            Debug.Log("붙잡기");
        }
    }

    public void SmellStickOn(SelectExitEventArgs arg)
    {
        SmellStick.OnEffect();
    }

    public void SmellStickOff(SelectEnterEventArgs arg)
    {
        SmellStick.OffEffect();
        
    }

    IEnumerator SocketActiveRoutine()
    {
        socket.allowSelect = true;
        yield return new WaitForSeconds(0.5f);
        socket.allowSelect = false;
    }
}

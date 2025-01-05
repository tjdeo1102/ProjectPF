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
        
    }


    public void SmellStickOn(SelectExitEventArgs arg)
    {
        SmellStick.OnEffect();
    }

    public void SmellStickOff(SelectEnterEventArgs arg)
    {
        SmellStick.OffEffect();
        SmellStick.NoteType = this.NoteType;
    }
}

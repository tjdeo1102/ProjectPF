using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;


public class WGH_InteractionNote : MonoBehaviourPun
{
    public E_WGH_NoteType NoteType;
    public WGH_SmellStick SmellStick;
    public XRSocketInteractor Socket;
    public WGH_NPCController Customer;
    public ParticleSystem PongEffect;
    private void Awake()
    {
        Socket = GetComponent<XRSocketInteractor>();
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
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Test1);
        photonView.RPC("OnPongEffect", RpcTarget.All);
        photonView.RPC("ChangeNote", RpcTarget.All);
    }

    [PunRPC]
    private void ChangeNote()
    {
        SmellStick.NoteType = this.NoteType;
    }

    [PunRPC]
    private void OnPongEffect()
    {
        PongEffect.Play();
    }
}

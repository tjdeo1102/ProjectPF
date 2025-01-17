using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabWhiteBoard : MonoBehaviourPun
{
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;
    [SerializeField] public Transform spawnTransform;

    bool setLabel = false;

    private void Start()
    {
        enabled = false;
    }

    public void Sound()
    {
        Debug.Log("grabwhiteboardsound");
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Label_on);
    }

    //[PunRPC]
    //public void RPC_Sound()
    //{
    //    KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Label_on);
    //}

    public void SetLabel()
    {
        if (setLabel == false)
        {
            boxCollider.enabled = true;
            meshCollider.enabled = false;
            setLabel = true;
            PhotonNetwork.Instantiate("Label_", spawnTransform.position, new Quaternion(0, -0.611637473f, 0, 0.791138232f));
        }
    }

}

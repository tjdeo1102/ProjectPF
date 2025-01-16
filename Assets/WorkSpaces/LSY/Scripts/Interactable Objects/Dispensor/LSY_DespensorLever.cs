using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class LSY_DespensorLever : MonoBehaviourPun, IPunObservable
{
    public LSY_DispensorLiquid dispensorLiquid;
    public GameObject lever;
    public XRLever xrLever;
    private bool isPlay = false;
    private Quaternion originalRotation;
    public float resetDuration = 1f;

    public bool isLeverEnabled = true; 

    private void Start()
    {
        originalRotation = lever.transform.rotation;
    }

    private void Update()
    {
        if (Mathf.Approximately(lever.transform.eulerAngles.x, 90f) && !isPlay)
        {
            isPlay = true;
            dispensorLiquid.OnSelectEnter();
            photonView.RPC("Sound", RpcTarget.All);
            photonView.RPC("LeverState", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void Sound()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.DispenserLever2);
    }

    public void StartRoutine()
    {
       StartCoroutine(ResetRotation());
    }

    private IEnumerator ResetRotation()
    {
        float time = 0f;
        Quaternion startRotation = lever.transform.rotation;
        photonView.RPC("Sound", RpcTarget.All);
        while (time < resetDuration)
        {
            lever.transform.rotation = Quaternion.Lerp(startRotation, originalRotation, time / resetDuration);
            time += Time.deltaTime;
            yield return null;
        }

        lever.transform.rotation = originalRotation;
        isPlay = false;

        photonView.RPC("LeverState", RpcTarget.All, true);
    }

    [PunRPC]
    public void LeverState(bool state)
    {
        isLeverEnabled = state;
        xrLever.enabled = state;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isPlay);
            stream.SendNext(isLeverEnabled);
        }
        else
        {
            isPlay = (bool)stream.ReceiveNext();
            isLeverEnabled = (bool)stream.ReceiveNext();
            xrLever.enabled = isLeverEnabled;
        }
    }
}

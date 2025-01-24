using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_Label : MonoBehaviourPun
{
    [SerializeField] Button doneButton;
    [SerializeField] Transform setPoint;
    [SerializeField] LSY_GrabWhiteBoard grabWhiteBoard;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        doneButton.onClick.AddListener(DoneButton);
        doneButton.gameObject.SetActive(true);
    }

    public void OnWhiteBoard()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        doneButton.gameObject.SetActive(true);
    }

    public void DoneButton()
    {
        photonView.RPC("RPC_DoneButton", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_DoneButton()
    {
        gameObject.transform.localScale = new Vector3(0.3038756f, 0.3038756f, 0.3038756f);
        gameObject.transform.position = setPoint.position;
        gameObject.transform.rotation = setPoint.rotation;

        doneButton.gameObject.SetActive(false);
        grabWhiteBoard.enabled = true;
    }

}

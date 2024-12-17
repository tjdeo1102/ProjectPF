using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_DirectInteractor : XRDirectInteractor
{
    [SerializeField] PhotonView photonView;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // ���� ����� ��Ʈ��ũ�� ���ؼ� ����
        // ���� �÷��̾ ���� ����� ��Ʈ��ũ�� ���� ����
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.RequestOwnership();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // ���� �÷��̾ ���� ��ü�� �������� ���忡�� �ٽ� �ֱ�
        PhotonView interactablePV = args.interactableObject.transform.GetComponent<PhotonView>();
        interactablePV.TransferOwnership(PhotonNetwork.MasterClient);
    }
}

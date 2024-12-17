using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_PlayerController : MonoBehaviourPun
{
    [SerializeField] Camera camera;
    [SerializeField] AudioListener audioListener;
    [SerializeField] TrackedPoseDriver trackedPoseDriver;
    [SerializeField] ActionBasedController leftController;
    [SerializeField] ActionBasedController rightController;

    private void Start()
    {
        if (photonView.IsMine == false)
        {
            camera.enabled = false;
            audioListener.enabled = false;
            trackedPoseDriver.enabled = false;
            leftController.enabled = false;
            rightController.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;

public class PTK_Player : MonoBehaviourPun
{
    [SerializeField] Camera camera;
    [SerializeField] AudioListener audioListener;
    [SerializeField] TrackedPoseDriver trackedPoseDriver;

    [SerializeField] ActionBasedController leftController;
    [SerializeField] ActionBasedController rightController;
    [SerializeField] ActionBasedControllerManager leftControllerManager;
    [SerializeField] ActionBasedControllerManager rightControllerManager;

    private void Awake()
    {
        if (photonView.IsMine == false)
        {
            camera.enabled = false;
            audioListener.enabled = false;
            trackedPoseDriver.enabled = false;
            leftController.enabled = false;
            rightController.enabled = false;
            leftControllerManager.enabled = false;
            rightControllerManager.enabled = false;
        }
    }
}

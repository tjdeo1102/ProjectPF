using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class KSH_PlayerContorller : MonoBehaviourPun
{
    [SerializeField] Camera cam;
    [SerializeField] AudioListener audioListener;
    [SerializeField] TrackedPoseDriver trackedPoseDriver;

    [SerializeField] ActionBasedController leftController;
    [SerializeField] ActionBasedController rightController;
    [SerializeField] ActionBasedControllerManager leftControllerManager;
    [SerializeField] ActionBasedControllerManager rightControllerManager;
    private void Awake()
    {
        if (photonView.Owner.IsLocal == false)
        {
            cam.enabled = false;
            audioListener.enabled = false;
            trackedPoseDriver.enabled = false;
            leftController.enabled = false;
            rightController.enabled = false;
            leftControllerManager.enabled = false;
            rightControllerManager.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class KSH_PlayerContorller : MonoBehaviourPun
{
    [SerializeField] Camera cam;
    [SerializeField] AudioListener audioListener;
    [SerializeField] TrackedPoseDriver trackedPoseDriver;

    [SerializeField] ActionBasedController leftController;
    [SerializeField] ActionBasedController rightController;
    private void Awake()
    {
        if (photonView.Owner.IsLocal == false)
        {
            cam.enabled = false;
            audioListener.enabled = false;
            trackedPoseDriver.enabled = false;
            leftController.enabled = false;
            rightController.enabled = false;
        }
    }
}

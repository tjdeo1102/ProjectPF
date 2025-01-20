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
    [SerializeField] GameObject locomotonObject;
    [SerializeField] SkinnedMeshRenderer skinRenderer;

    [SerializeField] SkinnedMeshRenderer skinRendererLHand;
    [SerializeField] SkinnedMeshRenderer skinRendererRHand;

    [SerializeField] ActionBasedController leftController;
    [SerializeField] ActionBasedController rightController;
    [SerializeField] ActionBasedControllerManager leftControllerManager;
    [SerializeField] ActionBasedControllerManager rightControllerManager;
    [SerializeField] GameObject soundUI;

    [SerializeField] private Animator animator;

    private Vector3 previousPosition;

    private float movementSpeed;

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
            locomotonObject.SetActive(false);
            skinRenderer.enabled = true;
            skinRendererLHand.enabled = false;
            skinRendererRHand.enabled = false;
            soundUI.SetActive(false);
        }
    }


    private void Start()
    {
        previousPosition = transform.position;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            UpdateMovementSpeed();
            UpdateAnimator();
        }
    }

    private void UpdateMovementSpeed()
    {
        Vector3 currentPosition = transform.position;
        Vector3 deltaPosition = currentPosition - previousPosition;

        movementSpeed = deltaPosition.magnitude / Time.deltaTime;

        previousPosition = currentPosition;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", movementSpeed);
        photonView.RPC("SyncAnimatorSpeed", RpcTarget.Others, movementSpeed);
    }

    [PunRPC]
    private void SyncAnimatorSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSD_Tooltip : MonoBehaviour
{
    [Header("설명 텍스트 추가")]
    [SerializeField] private GameObject tooltipCanvas;

    private Transform targetCam;
    private XRBaseInteractable interactable;

    private void Awake()
    {
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        targetCam = Camera.main.transform;
        interactable = GetComponent<XRBaseInteractable>();
        tooltipCanvas.SetActive(false);
        interactable.hoverEntered.AddListener(OnTooltip);
        interactable.hoverExited.AddListener(OffTooltip);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnTooltip);
        interactable.hoverExited.RemoveListener(OffTooltip);
    }

    public void OnTooltip(HoverEnterEventArgs args)
    {
        tooltipCanvas.SetActive(true);
    }
    public void OffTooltip(HoverExitEventArgs args)
    {
        tooltipCanvas.SetActive(false);
    }

    private void Update()
    {
        // 툴팁이 활성화된 동안에는 메인카메라로 텍스트가 바라보도록 설정
        if (targetCam == null) return;

        if (tooltipCanvas.activeSelf)
        {
            tooltipCanvas.transform.LookAt(targetCam.transform.position);

            var rot = tooltipCanvas.transform.eulerAngles;
            rot.y += 180f;

            tooltipCanvas.transform.eulerAngles = rot;
        }
    }
}

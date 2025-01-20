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
    }

    private void Update()
    {
        // 툴팁이 활성화된 동안에는 메인카메라로 텍스트가 바라보도록 설정
        if (targetCam == null) return;

        // 인터렉터가 호버면서 선택하지 않을 때만 툴팁 표시
        if (interactable.isHovered == true
            && interactable.isSelected == false)
        {
            tooltipCanvas.SetActive(true);
        }
        else
        {
            tooltipCanvas.SetActive(false);
            // 툴팁 위치 계산 필요없으므로 리턴
            return;
        }

        if (tooltipCanvas.activeSelf)
        {
            tooltipCanvas.transform.LookAt(targetCam.transform.position);

            var rot = tooltipCanvas.transform.eulerAngles;
            rot.y += 180f;

            tooltipCanvas.transform.eulerAngles = rot;
        }
    }
}

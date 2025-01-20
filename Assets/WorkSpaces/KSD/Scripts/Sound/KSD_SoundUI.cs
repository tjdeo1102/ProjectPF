using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KSD_SoundUI : MonoBehaviour
{
    [Header("키 입력 세팅")]
    [SerializeField] InputActionReference leftActiveButton;
    [SerializeField] InputActionReference rightActiveButton;

    private bool isPress;

    [Header("메인 캔버스")]
    [SerializeField] GameObject canvas;
    private float delayTimer = 1f;
    private float timer = 1f;

    private void OnEnable()
    {
        leftActiveButton.action.performed += Action_performed;
        rightActiveButton.action.performed += Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        isPress = !isPress;
    }

    private void Update()
    {
        if (timer < 0.01f)
        {
            if (isPress)
            {
                canvas.SetActive(!canvas.activeSelf);
                timer = delayTimer;
                isPress = false;
            }
        }

        timer -= Time.deltaTime;
        if (timer < 0f) timer = 0f;
    }

    
    private void OnDisable()
    {
        // 키입력 버튼
        leftActiveButton.action.performed -= Action_performed;
        rightActiveButton.action.performed -= Action_performed;
    }
}

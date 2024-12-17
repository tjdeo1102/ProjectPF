using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.CullingGroup;

public class UIController : MonoBehaviour
{
    [SerializeField] InputActionProperty rightJoystickInput;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] int itemCount;

    private void Start()
    {
        itemCount = 0;
    }

    private void Update()
    {
        var input = inputActions.actionMaps[0].actions[0].ReadValue<Vector2>().x;
        Debug.Log(input);
        if (Mathf.Abs(input) > 0)
        {
            // 물건의 갯수 늘어남
            itemCount++;
        }
        else
        {
            // 물건 갯수 줄어듬
                itemCount--;
        }
    }
    private void OnEnable()
    {
        rightJoystickInput.action.performed += MoveJoystick;
        rightJoystickInput.action.canceled += MoveJoystick;
    }

    private void OnDisable()
    {
        rightJoystickInput.action.performed -= MoveJoystick;
        rightJoystickInput.action.canceled -= MoveJoystick;
    }

    private void MoveJoystick(InputAction.CallbackContext context)
    {
        Vector2 joystickVector = context.ReadValue<Vector2>();
        Debug.Log(joystickVector);

        if (Mathf.Abs(joystickVector.x) > 0)
        {
            // 물건의 갯수 늘어남
        }
        else
        {
            // 물건 갯수 줄어듬
        }


        if (Mathf.Abs(joystickVector.y) > 0)
        {
            // 물건 위로 선택
        }
        else
        {
            // 물건 아래로 선택
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_PasswordPopUp : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] public TMP_Text roomNameText;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text notPasswordText;

    public delegate void OnPasswordSubmitDelegate(string password);
    public event OnPasswordSubmitDelegate OnPasswordSubmitEvent;

    public delegate void OnPasswordCancelDelegate();
    public event OnPasswordCancelDelegate OnPasswordCancelEvent;

    private void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnSubmit()
    {
        string enteredPassword = passwordInputField.text;
        InputFieldEmpty();
        OnPasswordSubmitEvent?.Invoke(enteredPassword);
    }

    private void OnCancel()
    {
        InputFieldEmpty();
        OnPasswordCancelEvent?.Invoke();
    }

    public void ClearInputField()
    {
        InputFieldEmpty();
        StartCoroutine(NotPasswordRoutine());
    }

    public void InputFieldEmpty()
    {
        passwordInputField.text = "";
    }

    IEnumerator NotPasswordRoutine()
    {
        notPasswordText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        notPasswordText.gameObject.SetActive(false);
    }
}

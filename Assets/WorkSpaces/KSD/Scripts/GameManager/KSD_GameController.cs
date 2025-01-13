using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KSD_GameController : MonoBehaviour
{
    public enum CurrentGameState
    {
        Open, BreakTime, Close, Null
    }
    [SerializeField] private TMP_Text closeText;
    [SerializeField] private XRBaseInteractable closeInteractable;

    private CurrentGameState currentGameState;
    private bool canExitGame;

    private void Start()
    {
        currentGameState = CurrentGameState.Open;
        KSD_GameManager.Instance.OnCanExitStage.AddListener(OnChangeStateHandler);
    }

    private void OnChangeStateHandler()
    {
        canExitGame = true;
        closeText.gameObject.SetActive(true);
        // Close 팻말을 잡아당긴것은 영수증을 발급받은 후이므로, 해당 Close를 하면 맵 저장과 함께 종료됨

        closeInteractable.selectEntered.AddListener((args) => KSD_GameManager.Instance.Quit(true,true,true));
    }

    
}

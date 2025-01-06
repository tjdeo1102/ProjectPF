using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KSD_CheatUI : MonoBehaviour
{
    public enum Panel { General, NPC, Teleport, Reset, Null }
    [SerializeField] GameObject generalPanel;
    [SerializeField] GameObject npcPanel;
    [SerializeField] GameObject teleportPanel;
    [SerializeField] GameObject resetPanel;

    [SerializeField] Button generalButton;
    [SerializeField] Button npcButton;
    [SerializeField] Button teleportButton;
    [SerializeField] Button resetButton;
    [SerializeField] Button closeButton;

    [Header("일반 설정")]
    [SerializeField] Button alwaysFire;
    [SerializeField] Button alwaysShake;
    [SerializeField] Button addDay;

    [Header("NPC")]
    [SerializeField] Button comeCounter;
    [SerializeField] Button addFinishNPC;

    [Header("텔레포트")]
    [SerializeField] Transform playerObject;
    [SerializeField] Button goMaterial;
    [SerializeField] Button goDispensor;
    [SerializeField] Button goCounter;

    [Header("리셋")]
    [SerializeField] Button returnLobby;
    [SerializeField] Button refreshGame;

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

    private void Start()
    {
        // 패널 전환 버튼
        generalButton.onClick.AddListener(GeneralButton);
        npcButton.onClick.AddListener(NPCButton);
        teleportButton.onClick.AddListener(TeleportButton);
        resetButton.onClick.AddListener(ResetButton);
        closeButton.onClick.AddListener(CloseButton);

        // 일반 설정 버튼
        alwaysFire.onClick.AddListener(() => KSD_CheatManager.Instance.IsAlwaysFire = !KSD_CheatManager.Instance.IsAlwaysFire);
        alwaysShake.onClick.AddListener(() => KSD_CheatManager.Instance.IsAlwaysShake = !KSD_CheatManager.Instance.IsAlwaysShake);
        addDay.onClick.AddListener(() => KSD_CheatManager.Instance.AddDay());

        // NPC 버튼
        comeCounter.onClick.AddListener(() => KSD_CheatManager.Instance.GoCounter());
        addFinishNPC.onClick.AddListener(() => KSD_CheatManager.Instance.AddFinishNPC());

        // 텔레포트 버튼
        goMaterial.onClick.AddListener(() => KSD_CheatManager.Instance.Teleport(playerObject, KSD_CheatManager.TeleportSpot.PerfumeMaterial));
        goDispensor.onClick.AddListener(() => KSD_CheatManager.Instance.Teleport(playerObject, KSD_CheatManager.TeleportSpot.Dispensor));
        goCounter.onClick.AddListener(() => KSD_CheatManager.Instance.Teleport(playerObject, KSD_CheatManager.TeleportSpot.Counter));

        // 리셋 버튼
        returnLobby.onClick.AddListener(() => KSD_CheatManager.Instance.ReturnLobby());
        refreshGame.onClick.AddListener(() => KSD_CheatManager.Instance.ReturnGame());
    }

    private void OnDisable()
    {
        // 패널 전환 버튼
        generalButton.onClick.RemoveListener(GeneralButton);
        npcButton.onClick.RemoveListener(NPCButton);
        teleportButton.onClick.RemoveListener(TeleportButton);
        resetButton.onClick.RemoveListener(ResetButton);
        closeButton.onClick.RemoveListener(CloseButton);

        // 일반 설정 버튼
        alwaysFire.onClick.RemoveAllListeners();
        alwaysShake.onClick.RemoveAllListeners();
        addDay.onClick.RemoveAllListeners();

        // NPC 버튼
        comeCounter.onClick.RemoveAllListeners();
        addFinishNPC.onClick.RemoveAllListeners();

        // 텔레포트 버튼
        goMaterial.onClick.RemoveAllListeners();
        goDispensor.onClick.RemoveAllListeners();
        goCounter.onClick.RemoveAllListeners();

        // 리셋 버튼
        returnLobby.onClick.RemoveAllListeners();
        refreshGame.onClick.RemoveAllListeners();

        // 키입력 버튼
        leftActiveButton.action.performed -= Action_performed;
        rightActiveButton.action.performed -= Action_performed;
    }

    private void SetActivePanel(Panel panel)
    {
        generalPanel.SetActive(panel == Panel.General);
        npcPanel.SetActive(panel == Panel.NPC);
        teleportPanel.SetActive(panel == Panel.Teleport);
        resetPanel.SetActive(panel == Panel.Reset);
    }

    public void CloseButton()
    {
        SetActivePanel(Panel.Null);
        canvas.SetActive(false);
    }

    public void GeneralButton()
    {
        SetActivePanel(Panel.General);
    }

    public void NPCButton()
    {
        SetActivePanel(Panel.NPC);
    }

    public void TeleportButton()
    {
        SetActivePanel(Panel.Teleport);
    }

    public void ResetButton()
    {
        SetActivePanel(Panel.Reset);
    }
}

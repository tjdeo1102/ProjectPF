using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_Kiosk : MonoBehaviourPun
{
    public enum Panel { shopPanel, optionPanel, quitPanel }

    public Button shopButton;
    public Button optionButton;
    public Button quitButton;

    public GameObject shopPanel;
    public GameObject optionPanel;
    public GameObject quitPanel;

    public Button shopBack;
    public Button optionBack;
    public Button quitBack;

    public Button quitgGameButton;

    private void Start()
    {
        shopButton.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.shopPanel, true));
        optionButton.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.optionPanel, true));
        quitButton.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.quitPanel, true));

        shopBack.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.shopPanel, false));
        optionBack.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.optionPanel, false));
        quitBack.onClick.AddListener(() => photonView.RPC("ActivePanel", RpcTarget.All, Panel.quitPanel, false));

        quitgGameButton.onClick.AddListener(QuitGame);
    }

    [PunRPC]
    public void ActivePanel(Panel panel, bool on)
    {
        switch (panel)
        {
            case Panel.shopPanel:
                shopPanel.SetActive(on);
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Tablet_on);
                break;
            case Panel.optionPanel:
                optionPanel.SetActive(on);
                break;
            case Panel.quitPanel:
                quitPanel.SetActive(on);
                break;
        }
    }

    private void QuitGame()
    {
        if (KSD_GameManager.Instance != null)
            KSD_GameManager.Instance.Quit(true,true,false);
    }
}

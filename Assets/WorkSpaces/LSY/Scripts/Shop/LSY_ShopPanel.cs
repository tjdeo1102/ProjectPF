using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ShopPanel : MonoBehaviourPun
{
    public enum Panel { Decoration, Furniture, Basket }
    [SerializeField] GameObject decorationPanel;
    [SerializeField] GameObject basketPanel;

    [SerializeField] Button decorationButton;
    [SerializeField] Button basketButton;
    [SerializeField] Button closeButton;

    private void Start()
    { 
        decorationButton.onClick.AddListener(DecorationButton);
        basketButton.onClick.AddListener(BasketButton);
        closeButton.onClick.AddListener(CloseButton);
    }

    [PunRPC]
    private void SetActivePanel(Panel panel)
    {
        decorationPanel.SetActive(panel == Panel.Decoration);
        basketPanel.SetActive(panel == Panel.Basket);
    }

    public void CloseButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Decoration);
    }

    public void DecorationButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Decoration);
    }

    public void BasketButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Basket);
    }
}

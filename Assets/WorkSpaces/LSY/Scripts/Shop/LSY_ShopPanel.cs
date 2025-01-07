using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ShopPanel : MonoBehaviourPun
{
    public enum Panel { Decoration, Furniture, Basket }
    [SerializeField] GameObject decorationPanel;
    [SerializeField] GameObject furniturePanel;
    [SerializeField] GameObject basketPanel;

    [SerializeField] Button decorationButton;
    [SerializeField] Button furnitureButton;
    [SerializeField] Button basketButton;
    [SerializeField] Button closeButton;

    private void Start()
    { 
        decorationButton.onClick.AddListener(DecorationButton);
        furnitureButton.onClick.AddListener(FurnitureButton);
        basketButton.onClick.AddListener(BasketButton);
        closeButton.onClick.AddListener(CloseButton);
    }

    [PunRPC]
    private void SetActivePanel(Panel panel)
    {
        decorationPanel.SetActive(panel == Panel.Decoration);
        furniturePanel.SetActive(panel == Panel.Furniture);
        basketPanel.SetActive(panel == Panel.Basket);
    }

    public void CloseButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Decoration);
        decorationPanel.SetActive(false);
    }

    public void DecorationButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Decoration);
    }

    public void FurnitureButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Furniture);
    }

    public void BasketButton()
    {
        photonView.RPC("SetActivePanel", RpcTarget.All, Panel.Basket);
    }
}

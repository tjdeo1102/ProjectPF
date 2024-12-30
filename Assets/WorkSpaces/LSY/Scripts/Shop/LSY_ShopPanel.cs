using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ShopPanel : MonoBehaviour
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

    private void SetActivePanel(Panel panel)
    {
        decorationPanel.SetActive(panel == Panel.Decoration);
        furniturePanel.SetActive(panel == Panel.Furniture);
        basketPanel.SetActive(panel == Panel.Basket);
    }

    public void CloseButton()
    {
        SetActivePanel(Panel.Decoration);
        decorationPanel.SetActive(false);
    }

    public void DecorationButton()
    {
        SetActivePanel(Panel.Decoration);
    }

    public void FurnitureButton()
    {
        SetActivePanel(Panel.Furniture);
    }

    public void BasketButton()
    {
        SetActivePanel(Panel.Basket);
    }
}

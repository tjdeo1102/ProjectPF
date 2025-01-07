using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_BasketPanel : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] Button deleteButton;

    [Header("아이템")]
    [SerializeField] string itemName;
    [SerializeField] float itemPrice;
    [SerializeField] string itemExplain;
    [SerializeField] Sprite item;
    [SerializeField] GameObject itemGameObekct;

    [Header("UI")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemExplainText;

    public delegate void ItemDelete(float price, string itemName);
    public event ItemDelete OnItemDelete;

    public delegate void ItemAdded(float price);
    public event ItemAdded OnItemAdded;

    float totalItemPrice;

    private void Start()
    {
        itemImage.sprite = item;
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemExplainText.text = itemExplain;

        deleteButton.onClick.AddListener(Delete);
    }

    public void SetItemInfo(string name, float price, string explanation, Sprite image, GameObject itemPrefab)
    {
        itemName = name;
        itemPrice = price;
        itemExplain = explanation;
        item = image;
        itemGameObekct = itemPrefab;

        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemExplainText.text = itemExplain;

        itemImage.sprite = item;
    }
    private void TotalPrice()
    {
        totalItemPrice = itemPrice;
    }

    private void Delete()
    {
        OnItemDelete?.Invoke(itemPrice, itemName);
        Destroy(gameObject);
    }
}

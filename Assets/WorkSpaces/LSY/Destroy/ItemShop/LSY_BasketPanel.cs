using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_BasketPanel : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] Button minusButton;
    [SerializeField] Button plusButton;
    [SerializeField] Button deleteButton;

    [Header("아이템 이름 및 가격")]
    [SerializeField] string itemName;
    [SerializeField] float itemPrice;

    [Header("텍스트")]
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] TextMeshProUGUI totalItemPriceText;

    public delegate void ItemDelete(float totalPrice);
    public event ItemDelete OnItemDelete;

    public delegate void ItemAdded(float price);
    public event ItemAdded OnItemAdded;

    int itemCount;
    float totalItemPrice;

    private void Start()
    {
        minusButton.onClick.AddListener(MinusCount);
        plusButton.onClick.AddListener(PlusCount);
        deleteButton.onClick.AddListener(Delete);
    }

    public void SetItemInfo(string name, float price, int count)
    {
        itemName = name;
        itemPrice = price;
        itemCount = count;

        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemCountText.text = itemCount.ToString();
        TotalPrice();
    }


    private void MinusCount()
    {
        if (itemCount < 1) return;

        OnItemAdded(-itemPrice);
        itemCount--;
        itemCountText.text = itemCount.ToString();

        TotalPrice();
    }

    private void PlusCount()
    {
        if (itemCount >= 10) return;

        OnItemAdded(+itemPrice);
        itemCount++;
        itemCountText.text = itemCount.ToString();

        TotalPrice();
    }

    private void TotalPrice()
    {
        totalItemPrice = itemPrice * itemCount;
        totalItemPriceText.text = totalItemPrice.ToString() + "$";
    }

    private void Delete()
    {
        OnItemDelete?.Invoke(totalItemPrice);
        Destroy(gameObject);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemPanel : MonoBehaviour
{
    [SerializeField] Button minusButton;
    [SerializeField] Button plusButton;
    [SerializeField] Button addButton;

    [Header("아이템 이름 및 가격")]
    [SerializeField] string itemName;
    [SerializeField] int itemPrice;

    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] TextMeshProUGUI totalItemPriceText;

    public delegate void ItemAdded(int price);
    public event ItemAdded OnItemAdded;

    int itemCount;
    int totalItemPrice;

    private void Start()
    {
        minusButton.onClick.AddListener(MinusCount);
        plusButton.onClick.AddListener(PlusCount);
        addButton.onClick.AddListener(AddItem);

        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString();

        itemCount = 0;
        itemCountText.text = itemCount.ToString();

        TotalPrice();
    }

    public void SetItemInfo(string name, int price)
    {
        itemName = name;
        itemPrice = price;

        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";

        itemCount = 0;
        itemCountText.text = itemCount.ToString();
        TotalPrice();
    }

    private void AddItem()
    {
        totalItemPrice = itemPrice * itemCount;

        OnItemAdded?.Invoke(totalItemPrice);

        itemCount = 0;
        itemCountText.text = itemCount.ToString();
        totalItemPrice = 0;
        totalItemPriceText.text = totalItemPrice.ToString() + "$";
    }

    private void MinusCount()
    {
        if (itemCount < 1) return;

        itemCount--;
        itemCountText.text = itemCount.ToString();

        TotalPrice();
    }

    private void PlusCount()
    {
        if (itemCount >= 10) return;

        itemCount++;
        itemCountText.text = itemCount.ToString();

        TotalPrice();
    }

    private void TotalPrice()
    {
        totalItemPrice = itemPrice * itemCount;
        totalItemPriceText.text = totalItemPrice.ToString() + "$";
    }
}

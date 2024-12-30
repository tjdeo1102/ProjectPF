using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemPanel : MonoBehaviour
{
    [SerializeField] Button addButton;

    [Header("æ∆¿Ã≈€")]
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

    public delegate void ItemAdded(float price);
    public event ItemAdded OnItemAdded;

    public delegate void ItemAddedBasket(string name, float price, string explain, Sprite sprite, GameObject itemPrefab);
    public event ItemAddedBasket OnItemAddedBasket;

    private void Start()
    {
        addButton.onClick.AddListener(AddItem);

        itemImage.sprite = item;
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemExplainText.text = itemExplain;
    }

    private void AddItem()
    {
        OnItemAddedBasket?.Invoke(itemName, itemPrice, itemExplain, item, itemGameObekct);
        OnItemAdded?.Invoke(itemPrice);
    }
}

using Photon.Pun;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemPanel : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Button addButton;

    [Header("아이템")]
    [SerializeField] public string itemName;
    [SerializeField] float itemPrice;
    [SerializeField] string itemExplain;
    [SerializeField] Sprite item;
    [SerializeField] GameObject itemGameObekct;

    [Header("UI")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemExplainText;

    [SerializeField] LSY_ItemManager itemManager;
    public bool isAdded;

    public delegate void ItemAdded(float price);
    public event ItemAdded OnItemAdded;

    public delegate void ItemAddedBasket(string name, float price, string explain, Sprite sprite, GameObject itemPrefab);
    public event ItemAddedBasket OnItemAddedBasket;

    private void Start()
    {
        isAdded = false;
        addButton.onClick.AddListener(AddItem);

        itemImage.sprite = item;
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemExplainText.text = itemExplain;
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
    private void AddItem()
    {
        if (LSY_ItemManager.basketIndex > 9 || isAdded)
        {
            Debug.Log("장바구니의 갯수가 10개가 넘었거나 이미 담은 물건입니다.");
            return;
        }
        OnItemAddedBasket?.Invoke(itemName, itemPrice, itemExplain, item, itemGameObekct);
        OnItemAdded?.Invoke(itemPrice);
        isAdded = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isAdded);
        }
        else
        {
            isAdded = (bool)stream.ReceiveNext();
        }
    }
}

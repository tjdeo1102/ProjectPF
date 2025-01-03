using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class LSY_ItemManager : MonoBehaviour
{
    [Header("장바구니 아이템 개수")]
    [SerializeField] TMP_Text basketCount;
    [SerializeField] TMP_Text basketPanelCount;
    int basketIndex = 0;

    [Header("아이템 목록")]
    [SerializeField] GameObject[] decorationPrefabs;
    [SerializeField] GameObject[] furniturePrefabs;

    [Header("버튼")]
    [SerializeField] Button OrderButton;

    [Header("프리팹")]
    [SerializeField] GameObject basketPanelPrefab;

    [Header("Content")]
    [SerializeField] Transform decorationContent;
    [SerializeField] Transform furnitureContent;
    [SerializeField] Transform basketContent;

    [Header("패널 및 최종가격 텍스트")]
    [SerializeField] GameObject basketPanel;
    [SerializeField] TextMeshProUGUI allItemPriceText;

    [Header("구매 후 팝업창")]
    [SerializeField] GameObject buyPopUp;
    [SerializeField] TMP_Text playerMoney;
    [SerializeField] TMP_Text buyPrice;
    [SerializeField] TMP_Text totalPlayerMoney;

    [Header("구매 물품 스폰 위치")]
    public Vector3 objectTransform;

    [SerializeField] Image warningImage;

    bool isBasketPanelActive = false;
    float totalPrice = 0;

    private List<LSY_BasketItem> basketItems = new List<LSY_BasketItem>();

    private void Start()
    {
        basketPanelCount.text = "0";
        allItemPriceText.text = "0$";
        basketIndex = 0;
        basketCount.text = basketIndex.ToString();

        InitializeItemPrefabs();

        foreach (var itemPrefab in decorationPrefabs)
        {
            LSY_ItemPanel itemPanelScript = itemPrefab.GetComponent<LSY_ItemPanel>();
            itemPanelScript.OnItemAdded += UpdateTotalPrice;
            itemPanelScript.OnItemAddedBasket += UpdateItemAddBasket;
        }

        foreach (var itemPrefab in furniturePrefabs)
        {
            LSY_ItemPanel itemPanelScript = itemPrefab.GetComponent<LSY_ItemPanel>();
            itemPanelScript.OnItemAdded += UpdateTotalPrice;
            itemPanelScript.OnItemAddedBasket += UpdateItemAddBasket;
        }

        OrderButton.onClick.AddListener(Order);
    }

    private void InitializeItemPrefabs()
    {
        decorationPrefabs = new GameObject[decorationContent.childCount];
        for (int i = 0; i < decorationContent.childCount; i++)
        {
            decorationPrefabs[i] = decorationContent.GetChild(i).gameObject;
        }

        furniturePrefabs = new GameObject[furnitureContent.childCount];
        for (int i = 0; i < furnitureContent.childCount; i++)
        {
            furniturePrefabs[i] = furnitureContent.GetChild(i).gameObject;
        }
    }

    private void UpdateItemDelete(float price, string itemName)
    {
        LSY_BasketItem itemToRemove = basketItems.Find(item => item.ItemName == itemName);
        if (itemToRemove != null)
        {
            basketItems.Remove(itemToRemove);
        }

        totalPrice -= price;
        allItemPriceText.text = totalPrice.ToString() + "$";

        basketIndex--;
        basketCount.text = basketIndex.ToString();
        basketPanelCount.text = basketIndex.ToString();

    }

    private void UpdateItemAddBasket(string name, float price, string explain, Sprite sprite, GameObject itemPrefab)
    {
        if (basketIndex >= 10)
        {
            if (warningRoutine == null)
            warningRoutine = StartCoroutine(WarningRoutine());

            return;
        }

        GameObject basketPanel = Instantiate(basketPanelPrefab, basketContent);
        LSY_BasketPanel basketPanelScript = basketPanel.GetComponent<LSY_BasketPanel>();
        basketPanelScript.SetItemInfo(name, price, explain, sprite, itemPrefab);

        basketPanelScript.OnItemDelete += UpdateItemDelete;
        basketPanelScript.OnItemAdded += UpdateTotalPrice;

        basketIndex++;
        basketCount.text = basketIndex.ToString();
        basketPanelCount.text = basketIndex.ToString();

        basketItems.Add(new LSY_BasketItem(name, price, explain, sprite, itemPrefab));
    }
    Coroutine warningRoutine;
    IEnumerator WarningRoutine()
    {
        warningImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        warningImage.gameObject.SetActive(false);
        warningRoutine = null;
    }

    private void UpdateTotalPrice(float addedPrice)
    {
        totalPrice += addedPrice;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }

    private void Order()
    {
        if (basketItems.Count > 0)
        {
            StartCoroutine(BuyRoutine());

            foreach (var item in basketItems)
            {
                GameObject itemPrefab = item.ItemPrefab;
                Instantiate(itemPrefab, objectTransform, Quaternion.identity);
                Debug.Log($"주문한 아이템: {item.ItemName}, 가격: {item.ItemPrice}$");
            }

            Debug.Log("주문이 완료되었습니다.");
        }
        else
        {
            Debug.Log("장바구니에 아이템이 없습니다.");
        }
    }

    private void ClearBasket()
    {
        foreach (Transform child in basketContent)
        {
            Destroy(child.gameObject);
        }

        basketItems.Clear();
        basketIndex = 0;
        basketCount.text = basketIndex.ToString();
        basketPanelCount.text = basketIndex.ToString();
        totalPrice = 0;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }

    IEnumerator BuyRoutine()
    {
        buyPopUp.gameObject.SetActive(true);

        yield return null;
        playerMoney.text = "소지금: $" + "플레이어돈"; 
        yield return new WaitForSeconds(0.5f);

        buyPrice.text = "구매 금액: $" + totalPrice;
        yield return new WaitForSeconds(0.5f);

        totalPlayerMoney.text = "구매 후 금액: $" + ("플레이어돈 - " + totalPrice); 
        yield return new WaitForSeconds(5);

        buyPopUp.gameObject.SetActive(false);
        ClearBasket();
    }
}

public class LSY_BasketItem
{
    public string ItemName { get; private set; }
    public float ItemPrice { get; private set; }
    public string ItemExplain { get; private set; }
    public Sprite ItemSprite { get; private set; }
    public GameObject ItemPrefab { get; private set; }

    public LSY_BasketItem(string name, float price, string explain, Sprite sprite, GameObject prefab)
    {
        ItemName = name;
        ItemPrice = price;
        ItemExplain = explain;
        ItemSprite = sprite;
        ItemPrefab = prefab;
    }
}

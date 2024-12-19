using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemDictionary : SerializableDictionary<string, float> { }

public class LSY_ItemManager : MonoBehaviour
{
    [Header("아이템 목록")]
    public ItemDictionary items;

    [Header("버튼")]
    [SerializeField] Button OrderButton;
    [SerializeField] Button basketButton;

    [Header("프리팹")]
    [SerializeField] GameObject itemPanelPrefab;
    [SerializeField] GameObject basketPanelPrefab;

    [Header("Content")]
    [SerializeField] Transform itemContent;      
    [SerializeField] Transform basketContent;

    [Header("패널 및 최종가격 텍스트")]
    [SerializeField] GameObject basketPanel;
    [SerializeField] TextMeshProUGUI allItemPriceText; 

    bool isBasketPanelActive = false;
    float totalPrice = 0;

    float itemPrefabHeight;
    float basketPanelHeight;

    private void Start()
    {
        itemPrefabHeight = itemPanelPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        basketPanelHeight = basketPanelPrefab.gameObject.GetComponent<RectTransform>().rect.height;

        foreach (var item in items)
        {
            // 아이템이 늘어나면 Content의 Hight 높이도 늘어나도록 해줌
            RectTransform rectTransform = itemContent.gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + itemPrefabHeight + 5 );

            // 아이템 패널 프리팹을 Content의 자식오브젝트로 생성해줌
            GameObject itemPanel = Instantiate(itemPanelPrefab, itemContent);
            LSY_ItemPanel itemPanelScript = itemPanel.GetComponent<LSY_ItemPanel>();

            // 프리팹의 LSY_ItemPanel스트립트의 아이템이름과 값을 딕셔너리값으로 초기설정해줌
            itemPanelScript.SetItemInfo(item.Key, item.Value);

            // 아이템을 장바구니에 담으면 총 가격 변동을 위한 이벤트를 추가해줌
            itemPanelScript.OnItemAdded += UpdateTotalPrice;
            itemPanelScript.OnItemAddedBasket += UpdateItemAddBasket;

        }

        OrderButton.onClick.AddListener(Order);
        basketButton.onClick.AddListener(BasketButton);

        UpdateTotalPrice(0);
    }


    private void UpdateItemDelete(float price)
    {
        totalPrice -= price;
        allItemPriceText.text = totalPrice.ToString() + "$";
        RectTransform rectTransform = basketContent.gameObject.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - basketPanelHeight -5);
        Debug.Log(basketPanelPrefab.gameObject.GetComponent<RectTransform>().rect.height);
    }

    private void UpdateItemAddBasket(string name, float price, int count)
    {

        RectTransform rectTransform = basketContent.gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + basketPanelHeight + 5);

        GameObject basketPanel = Instantiate(basketPanelPrefab, basketContent);
        LSY_BasketPanel basketPanelScript = basketPanel.GetComponent<LSY_BasketPanel>();
        basketPanelScript.SetItemInfo(name, price, count);
        basketPanelScript.OnItemDelete += UpdateItemDelete;
        basketPanelScript.OnItemAdded += UpdateTotalPrice;

    }

    private void UpdateTotalPrice(float addedPrice)
    {
        totalPrice += addedPrice;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }

    private void Order()
    {
        // 플레이어의 돈에서 - totalPrice 해줘야 함
        // 재료 채워져야 함
    }

    private void BasketButton()
    {
        if (isBasketPanelActive)
        {
            basketPanel.SetActive(false);
        }
        else
        {
            basketPanel.SetActive(true);
        }

        isBasketPanelActive = !isBasketPanelActive;
    }
}

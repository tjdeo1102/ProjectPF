using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemDictionary : SerializableDictionary<string, float> { }

public class LSY_ItemManager : MonoBehaviour
{
    [Header("������ ���")]
    public ItemDictionary items;

    [Header("��ư")]
    [SerializeField] Button OrderButton;
    [SerializeField] Button basketButton;

    [Header("������")]
    [SerializeField] GameObject itemPanelPrefab;
    [SerializeField] GameObject basketPanelPrefab;

    [Header("Content")]
    [SerializeField] Transform itemContent;      
    [SerializeField] Transform basketContent;

    [Header("�г� �� �������� �ؽ�Ʈ")]
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
            // �������� �þ�� Content�� Hight ���̵� �þ���� ����
            RectTransform rectTransform = itemContent.gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + itemPrefabHeight + 5 );

            // ������ �г� �������� Content�� �ڽĿ�����Ʈ�� ��������
            GameObject itemPanel = Instantiate(itemPanelPrefab, itemContent);
            LSY_ItemPanel itemPanelScript = itemPanel.GetComponent<LSY_ItemPanel>();

            // �������� LSY_ItemPanel��Ʈ��Ʈ�� �������̸��� ���� ��ųʸ������� �ʱ⼳������
            itemPanelScript.SetItemInfo(item.Key, item.Value);

            // �������� ��ٱ��Ͽ� ������ �� ���� ������ ���� �̺�Ʈ�� �߰�����
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
        // �÷��̾��� ������ - totalPrice ����� ��
        // ��� ä������ ��
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

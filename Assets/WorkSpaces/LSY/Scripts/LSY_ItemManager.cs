using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemManager : MonoBehaviour
{
    [SerializeField] GameObject itemPanelPrefab; 
    [SerializeField] Transform uiParent;      
    [SerializeField] TextMeshProUGUI allItemPriceText; 
    [SerializeField] Button resetButton;      

    Dictionary<string, int> items;
    int totalPrice = 0; 

    private void Start()
    {
        items = new Dictionary<string, int>()
        {
            {"Banana", 3},
            {"Apple", 5},
            {"Grape", 2},
            {"Water", 1 },
            {"Donut", 10 },
            {"Orange", 6 },
            {"Pinut", 2 }
        };

        foreach (var item in items)
        {
            // �������� �þ�� Content�� Hight ���̵� �þ���� ����
            RectTransform rectTransform = uiParent.gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 155);

            // ������ �г� �������� Content�� �ڽĿ�����Ʈ�� ��������
            GameObject itemPanel = Instantiate(itemPanelPrefab, uiParent);
            LSY_ItemPanel itemPanelScript = itemPanel.GetComponent<LSY_ItemPanel>();

            // �������� LSY_ItemPanel��Ʈ��Ʈ�� �������̸��� ���� ��ųʸ������� �ʱ⼳������
            itemPanelScript.SetItemInfo(item.Key, item.Value);

            // �������� ��ٱ��Ͽ� ������ �� ���� ������ ���� �̺�Ʈ�� �߰�����
            itemPanelScript.OnItemAdded += UpdateTotalPrice;

        }

        resetButton.onClick.AddListener(ResetPrice);

        UpdateTotalPrice(0);
    }

    private void UpdateTotalPrice(int addedPrice)
    {
        totalPrice += addedPrice;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }

    private void ResetPrice()
    {
        totalPrice = 0;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }
}

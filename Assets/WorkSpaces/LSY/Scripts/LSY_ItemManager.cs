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
            // 아이템이 늘어나면 Content의 Hight 높이도 늘어나도록 해줌
            RectTransform rectTransform = uiParent.gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 155);

            // 아이템 패널 프리팹을 Content의 자식오브젝트로 생성해줌
            GameObject itemPanel = Instantiate(itemPanelPrefab, uiParent);
            LSY_ItemPanel itemPanelScript = itemPanel.GetComponent<LSY_ItemPanel>();

            // 프리팹의 LSY_ItemPanel스트립트의 아이템이름과 값을 딕셔너리값으로 초기설정해줌
            itemPanelScript.SetItemInfo(item.Key, item.Value);

            // 아이템을 장바구니에 담으면 총 가격 변동을 위한 이벤트를 추가해줌
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

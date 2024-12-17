using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_TotalPrice : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    public static int Price;

    [SerializeField] Button ResetButton;

    private void Start()
    {
        Price = 0;
        priceText.text = Price.ToString() + "$";

        ResetButton.onClick.AddListener(ResetPrice);
    }

    private void ResetPrice()
    {
        Price = 0;
        priceText.text = Price.ToString() + "$";
    }
}

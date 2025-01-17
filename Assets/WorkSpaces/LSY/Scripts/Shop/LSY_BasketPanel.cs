using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_BasketPanel : MonoBehaviourPun
{
    [Header("버튼")]
    [SerializeField] Button deleteButton;

    [Header("아이템")]
    [SerializeField] public string itemName;
    [SerializeField] float itemPrice;
    [SerializeField] string itemExplain;
    [SerializeField] Sprite item;
    [SerializeField] public GameObject itemGameObekct;

    [Header("UI")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemExplainText;

    public delegate void ItemDelete(float price, string itemName);
    public event ItemDelete OnItemDelete;

    float totalItemPrice;

    private void Start()
    {
        itemImage.sprite = item;
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString() + "$";
        itemExplainText.text = itemExplain;

        deleteButton.onClick.AddListener(Delete);
    }

    public void ButtonClick()
    {
        photonView.RPC("RPC_ButtonClick", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ButtonClick()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
    }

    private void TotalPrice()
    {
        totalItemPrice = itemPrice;
    }

    private void Delete()
    {
        OnItemDelete?.Invoke(itemPrice, itemName);
    }
}

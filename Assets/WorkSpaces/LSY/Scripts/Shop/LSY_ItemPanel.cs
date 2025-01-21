using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemPanel : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Button addButton;

    [Header("¾ÆÀÌÅÛ")]
    [SerializeField] public string itemName;
    [SerializeField] public float itemPrice;
    [SerializeField] public string itemExplain;
    [SerializeField] Sprite item;

    [Header("UI")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemExplainText;

    [Header("ÆË¾÷")]
    public GameObject popUp_Count;
    public GameObject popUp_OnBasket;

    [SerializeField] LSY_ItemManager itemManager;
    public bool isAdded;

    public delegate void ItemAdded(float price);
    public event ItemAdded OnItemAdded;

    public delegate void ItemAddedBasket(string name);
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

    public void ButtonClick()
    {
        photonView.RPC("RPC_ButtonClick", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ButtonClick()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Button1);
    }

    private void AddItem()
    {
        if (LSY_ItemManager.basketIndex > 9)
        {
            photonView.RPC("OnPopUp_Count", RpcTarget.All);
            return;
        }

        if (isAdded)
        {
            photonView.RPC("OnPopUp_OnBasket", RpcTarget.All);
            return;
        }
        OnItemAddedBasket?.Invoke(itemName);
        OnItemAdded?.Invoke(itemPrice);
        photonView.RPC("RPC_IsAdded", RpcTarget.All);
    }

    [PunRPC]
    public void OnPopUp_Count()
    {
        if (popUpRoutine == null)
        {
            popUpRoutine = StartCoroutine(PopUpRoutine());
        }
    }

    Coroutine popUpRoutine;
    IEnumerator PopUpRoutine()
    {
        popUp_Count.SetActive(true);
        yield return new WaitForSeconds(1);
        popUp_Count.SetActive(false);
        popUpRoutine = null;
    }

    [PunRPC]
    public void OnPopUp_OnBasket()
    {
        if (popUpRoutine_Basket == null)
        {
            popUpRoutine_Basket = StartCoroutine(PopUpRoutine_Basket());
        }
    }

    Coroutine popUpRoutine_Basket;
    IEnumerator PopUpRoutine_Basket()
    {
        popUp_OnBasket.SetActive(true);
        yield return new WaitForSeconds(1);
        popUp_OnBasket.SetActive(false);
        popUpRoutine_Basket = null;
    }


    [PunRPC]
    public void RPC_IsAdded()
    {
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

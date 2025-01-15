using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LSY_ItemManager : MonoBehaviourPun, IPunObservable
{
    [Header("장바구니 아이템 개수")]
    [SerializeField] TMP_Text basketCount;
    [SerializeField] TMP_Text basketPanelCount;
    static public int basketIndex = 0;

    [Header("아이템 목록")]
    [SerializeField] List<LSY_ItemPanel> itemPanels = new();
    [SerializeField] List<LSY_BasketPanel> basketItems = new List<LSY_BasketPanel>();
    [SerializeField] List<string> basketNames = new();

    [Header("버튼")]
    [SerializeField] Button OrderButton;

    [Header("Content")]
    [SerializeField] Transform decorationContent;
    [SerializeField] Transform basketContent;

    [Header("패널 및 최종가격 텍스트")]
    [SerializeField] GameObject basketPanel;
    [SerializeField] TextMeshProUGUI allItemPriceText;

    [Header("구매 후 팝업창")]
    [SerializeField] GameObject buyPopUp;
    [SerializeField] TMP_Text playerMoney;
    [SerializeField] TMP_Text buyPrice;
    [SerializeField] TMP_Text totalPlayerMoney;

    [Header("장바구니 경고창")]
    [SerializeField] Image warningImage;

    [Header("플레이어 돈")]
    [SerializeField] TMP_Text playerMoneyText;

    [SerializeField] GameObject order_PlayerMoney_PopUp;
    [SerializeField] GameObject order_BasketCount_PopUp;

    //[SerializeField] KSD_StageInfo stageInfo;

    float totalPrice = 0;

    //---------------------- 01.11 KSD 수정 --------------------- //
    private void Start()
    {
        if (KSD_GameManager.Instance == null) return;

        // 초기화 되는 타이밍을 OnChangeStageInfo으로 받아야 됨. (네트워크 안정화 이슈)
        KSD_GameManager.Instance.OnChangeStageInfo.AddListener(Initialize);
    }

    
    private void Initialize()
    {
        InitializeItemPanels(decorationContent);
        InitializeBasketPanels();

        playerMoneyText.text = "$" + KSD_GameManager.Instance.CurrentStageInfo.StageMoney;
        basketPanelCount.text = "0";
        allItemPriceText.text = "0$";
        basketIndex = 0;
        basketCount.text = basketIndex.ToString();

        OrderButton.onClick.AddListener(Order);

        KSD_GameManager.Instance.OnChangeStageInfo.RemoveListener(Initialize);
    }

    // 치트 모드용 돈 업데이트 UI 적용
    public void UpdateMoney()
    {
        playerMoneyText.text = "$" + KSD_GameManager.Instance.CurrentStageInfo.StageMoney;
    }
    //---------------------- 수정 --------------------- //

    private void InitializeItemPanels(Transform content)
    {
        foreach (Transform child in content)
        {
            LSY_ItemPanel itemPanel = child.GetComponent<LSY_ItemPanel>();
            if (itemPanel != null)
            {
                itemPanels.Add(itemPanel);
                itemPanel.gameObject.SetActive(true);
                itemPanel.OnItemAdded += (float price) => photonView.RPC("RPC_UpdateTotalPrice", RpcTarget.All, price);
                itemPanel.OnItemAddedBasket += UpdateItemAddBasket;

                foreach (var stageInfo in KSD_GameManager.Instance.CurrentStageInfo.BuyItems)
                {
                    if (stageInfo == itemPanel.itemName)
                    {
                        itemPanels.Remove(itemPanel);
                        itemPanel.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void InitializeBasketPanels()
    {
        foreach (Transform child in basketContent)
        {
            LSY_BasketPanel basketPanel = child.GetComponent<LSY_BasketPanel>();
            if (basketPanel != null)
            {
                basketItems.Add(basketPanel);
                child.gameObject.SetActive(false);
                basketPanel.OnItemDelete += (float price, string itemName) => photonView.RPC("RPC_UpdateItemDelete", RpcTarget.All, price, itemName);
            }
        }
    }

    [PunRPC]
    public void RPC_UpdateItemDelete(float price, string itemName)
    {
        LSY_BasketPanel itemToRemove = basketItems.Find(item => item.itemName == itemName);
        LSY_ItemPanel itemPanel = itemPanels.Find(panel => panel.itemName == itemName);

        if (itemToRemove != null)
        {
            itemToRemove.gameObject.SetActive(false);
        }

        if (itemPanel != null)
        {
            itemPanel.isAdded = false;
        }

        totalPrice -= price;
        allItemPriceText.text = totalPrice.ToString() + "$";

        basketIndex--;
        basketCount.text = basketIndex.ToString();
        basketPanelCount.text = basketIndex.ToString();
        basketNames.Remove(itemName);
    }

    private void UpdateItemAddBasket(string name)
    {
        if (basketIndex > 9)
        {
            if (warningRoutine == null)
                warningRoutine = StartCoroutine(WarningRoutine());

            return;
        }
        photonView.RPC("RPC_UpdateItemAddBasket", RpcTarget.All, name);
    }

    [PunRPC]
    public void RPC_UpdateItemAddBasket(string name)
    {
        basketIndex++;
        basketCount.text = basketIndex.ToString();
        basketPanelCount.text = basketIndex.ToString();
        LSY_BasketPanel basketItem = basketItems.Find(item => item.itemName == name);
        basketItem.gameObject.SetActive(true);
        basketNames.Add(name);
    }

    Coroutine warningRoutine;
    IEnumerator WarningRoutine()
    {
        warningImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        warningImage.gameObject.SetActive(false);
        warningRoutine = null;
    }

    [PunRPC]
    public void RPC_UpdateTotalPrice(float addedPrice)
    {
        totalPrice += addedPrice;
        allItemPriceText.text = totalPrice.ToString() + "$";
    }

    private void Order()
    {
        photonView.RPC("RPC_Order", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Order()
    {
        int count = 0;
        foreach (var basketItem in basketItems)
        {
            if (basketItem == null) continue;

            if (basketItem.gameObject.activeSelf)
            {
                Debug.Log("카운트");
                count++;
            }
        }

        if (count == 0)
        {
            if (orderPopUpRoutine == null)
            {
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Tablet_fall);
                orderPopUpRoutine = StartCoroutine(OrderPopUpRoutine(order_BasketCount_PopUp));
            }
            return;
        }

        if (KSD_GameManager.Instance.CurrentStageInfo.StageMoney < totalPrice)
        {
            if (orderPopUpRoutine == null)
            {
                KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Tablet_fall);
                orderPopUpRoutine = StartCoroutine(OrderPopUpRoutine(order_PlayerMoney_PopUp));
            }
            return;
        }

        if (basketItems.Count > 0)
        {
            KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Tablet_success);
            StartCoroutine(BuyRoutine());
        }
    }

    Coroutine orderPopUpRoutine;
    IEnumerator OrderPopUpRoutine(GameObject gameObject)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        orderPopUpRoutine = null;
    }

    private void ClearBasket()
    {
        foreach (var basketItem in basketNames)
        {
            KSD_GameManager.Instance.CurrentStageInfo.BuyItems.Add(basketItem);
        }

        foreach (var item in basketItems)
        {
            if (basketNames.Contains(item.itemName))
            {
                item.itemGameObekct.SetActive(true);
                Destroy(item.gameObject);
            }
        }

        foreach (var item in itemPanels)
        {
            if (basketNames.Contains(item.itemName))
            {
                Destroy(item.gameObject);
            }
        }

        basketNames.Clear();

        playerMoney.text = "소지금: ";
        buyPrice.text = "구매 금액: ";
        totalPlayerMoney.text = "구매 후 금액: ";

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
        playerMoney.text = "소지금: $" + KSD_GameManager.Instance.CurrentStageInfo.StageMoney;
        yield return new WaitForSeconds(0.5f);

        buyPrice.text = "구매 금액: $" + totalPrice;
        yield return new WaitForSeconds(0.5f);

        totalPlayerMoney.text = "구매 후 금액: $" + (KSD_GameManager.Instance.CurrentStageInfo.StageMoney - totalPrice);
        KSD_GameManager.Instance.CurrentStageInfo.StageMoney = KSD_GameManager.Instance.CurrentStageInfo.StageMoney - (int)totalPrice;

        playerMoneyText.text = "$" + KSD_GameManager.Instance.CurrentStageInfo.StageMoney;
        yield return new WaitForSeconds(5);

        buyPopUp.gameObject.SetActive(false);
        ClearBasket();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(basketIndex);
            stream.SendNext(KSD_GameManager.Instance.CurrentStageInfo.StageMoney);
            stream.SendNext(basketItems.Count);
        }
        else
        {
            basketIndex = (int)stream.ReceiveNext();
            KSD_GameManager.Instance.CurrentStageInfo.StageMoney = (int)stream.ReceiveNext();
            int basketItemCount = (int)stream.ReceiveNext();
        }
    }
}

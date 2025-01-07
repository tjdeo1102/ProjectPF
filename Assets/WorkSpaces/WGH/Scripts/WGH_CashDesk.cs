using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class WGH_CashDesk : MonoBehaviour
{
    private XRSocketInteractor socket;

    private GameObject npcObj;
    public WGH_NPCController Customer;

    public GameObject Perfume;

    [SerializeField] private Button purchaseButton;

    [SerializeField] private bool isCheck;
    [SerializeField, Tooltip("실패기준 횟수")] int maxCount;
    private int curCount;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        npcObj = GameObject.FindGameObjectWithTag("TestNote");

        socket.selectEntered.AddListener(FindCustomer);
        socket.selectExited.AddListener(DeleteCustomer);

        purchaseButton.onClick.AddListener(OnPurchase);
    }

    public void FindCustomer(SelectEnterEventArgs arg)
    {
        StartCoroutine(FindRoutine());
    }

    IEnumerator FindRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        // 손님 탐색
        Customer = npcObj.transform.GetChild(0).GetComponent<WGH_InteractionNote>().Customer;
        // 소켓에 끼워진 향수 탐색
        Perfume = socket.GetOldestInteractableSelected().transform.gameObject;
        yield break;
    }

    public void DeleteCustomer(SelectExitEventArgs arg)
    {
        // 손님 null
        Customer = null;
        // 향수 null
        Perfume = null;
    }

    public void OnPurchase()
    {
        if (Perfume != null && Perfume.TryGetComponent(out LSY_PotionReceiver perfume) && PhotonNetwork.IsMasterClient == true)
        {
            if (Perfume.GetComponent<LSY_PotionReceiver>().perfumeName == Customer.PerfumeType
                && Perfume.GetComponent<LSY_PotionReceiver>().e_BottleType == Customer.BottleType
                && isCheck == false && curCount < maxCount)
            {
                // 성공하면 성공 감정표현 후 퇴장
                StartCoroutine(PurchaseRoutine());
            }
            else if ((Perfume.GetComponent<LSY_PotionReceiver>().perfumeName != Customer.PerfumeType
                || Perfume.GetComponent<LSY_PotionReceiver>().e_BottleType != Customer.BottleType)
                && isCheck == false && curCount < maxCount)
            {
                // 실패하면 절망 감정표현 후 실패횟수 1회 추가
                // 실패횟수 2회 이상일 시 퇴장
                StartCoroutine(CheckTimeDelayRoutine());
            }
            else
            {
                Debug.Log("뭔가 잘못됨");
            }
        }
        else
        {
            Debug.Log("향수가 올려져 있지 않음");
        }
    }

    IEnumerator PurchaseRoutine()
    {
        isCheck = true;
        Customer.SelectReactUINetwork((int)E_ReactUiType.BEST);
        
        KSD_GameManager.Instance.AddFinishPlayerCount(1);
        yield return new WaitForSeconds(2);
        WGH_NPCCreator.Instance.isCounter = false;
        curCount = 0;
        Customer.ChangeStateNetwork((int)E_StateType.EXIT);
        yield return null;
        PhotonNetwork.Destroy(Perfume);
        yield return null;
        isCheck = false;
    }

    IEnumerator CheckTimeDelayRoutine()
    {
        isCheck = true;
        curCount++;
        Customer.SelectReactUINetwork((int)E_ReactUiType.DESPAIR);
        yield return new WaitForSeconds(2);
        if (curCount >= maxCount)
        {
            // 실패횟수가 설정된 수에 도달하면 퇴장
            WGH_NPCCreator.Instance.isCounter = false;
            curCount = 0;
            Customer.ChangeStateNetwork((int)E_StateType.EXIT);
            yield return null;
            PhotonNetwork.Destroy(Perfume);
            yield return null;
            isCheck = false;
        }
        else
        {
            isCheck = false;
        }
    }
}

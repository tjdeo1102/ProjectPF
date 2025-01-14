using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LSY_Receipt : MonoBehaviourPun
{
    [SerializeField] GameObject endPanel;

    [SerializeField] TMP_Text dayText;
    [SerializeField] TMP_Text visitorCountText;
    [SerializeField] TMP_Text perfumeNumberText;
    [SerializeField] TMP_Text revenueText;
    [SerializeField] TMP_Text closingtimeText;
    [SerializeField] TMP_Text distanceMovedText;

    WaitForSeconds delay = new WaitForSeconds(0.5f);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameEnd();
        }
    }

    public void GameEnd()
    {
        photonView.RPC("RPC_GameEnd", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_GameEnd()
    {
        KSH_AudioManager.Instance.PlaySfx(KSH_AudioManager.Sfx.Bill2);
        endPanel.SetActive(true);
        if (receiptRoutine == null)
        {
            receiptRoutine = StartCoroutine(ReceiptRoutine());
        }
    }

    Coroutine receiptRoutine;

    IEnumerator ReceiptRoutine()
    {
        dayText.text = "1일차";
        yield return new WaitForSeconds(1f);
        visitorCountText.text = "5명";
        yield return delay;
        perfumeNumberText.text = "3개";
        yield return delay;
        revenueText.text = "340$";
        yield return delay;
        closingtimeText.text = "5: 40초";
        yield return delay;
        distanceMovedText.text = "3522M";
    }

    public void Reset()
    {
        dayText.text = "";
        visitorCountText.text = "";
        perfumeNumberText.text = "";
        revenueText.text = "";
        closingtimeText.text = "";
        distanceMovedText.text = "";
    }

}

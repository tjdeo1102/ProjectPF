using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LSY_GrabLabel : MonoBehaviourPun
{
    public LSY_Label lsy_label;
    bool doneLabel = false;
    public GameObject label;
    public GameObject whiteBoard;

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(0.3f);
        label.SetActive(false);
        whiteBoard.SetActive(true);
        doneLabel = true;
    }

    public void Grab()
    {
        if (doneLabel) return;
        Debug.Log("grablabelsound");
        lsy_label.OnWhiteBoard();
        StartCoroutine(Routine());
    }

}

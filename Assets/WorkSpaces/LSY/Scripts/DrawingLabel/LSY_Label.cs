using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSY_Label : MonoBehaviour
{
    [SerializeField] Button doneButton;
    [SerializeField] Transform setPoint;
    [SerializeField] LSY_GrabWhiteBoard grabWhiteBoard;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        doneButton.onClick.AddListener(DoneButton);
        doneButton.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnWhiteBoard();
        }
    }

    public void OnWhiteBoard()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        doneButton.gameObject.SetActive(true);
    }

    public void DoneButton()
    {
        gameObject.transform.localScale = new Vector3(0.3038756f, 0.3038756f, 0.3038756f);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        gameObject.transform.position = setPoint.position;

        doneButton.gameObject.SetActive(false);
        grabWhiteBoard.enabled = true;
    }

}

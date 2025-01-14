using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using GogoGaga.OptimizedRopesAndCables;

public class WGH_NPCPause : MonoBehaviourPun
{
    [SerializeField] private GameObject open;
    private Rope rope;
    private Transform startPoint;
    private Transform endPoint;
    private Transform finishPoint;
    private GameObject grabObj;

    private Rope openRope;
    private Transform startPointOpen;
    private Transform endPointOpen;
    private Transform finishPointOpen;
    private GameObject openGrabObj;

    public UnityEvent OnPause;
    public UnityEvent OffPause;

    private bool isActive;
    private bool isActiveOpen;
    private bool isGrab;
    private bool isGrabOpen;

    private float distance;

    private void Start()
    {
        // BreakTime ¿ë
        rope = GetComponentInChildren<Rope>();
        startPoint = rope.transform.GetChild(0);
        endPoint = rope.transform.GetChild(1);
        finishPoint = rope.transform.GetChild(2);
        grabObj = endPoint.gameObject;
        // Open ¿ë
        openRope = open.GetComponentInChildren<Rope>();
        startPointOpen = openRope.transform.GetChild(0);
        endPointOpen = openRope.transform.GetChild(1);
        finishPointOpen = openRope.transform.GetChild(2);
        openGrabObj = endPointOpen.gameObject;

        distance = 2f;
    }

    private void Update()
    {
        if (isGrab)
        {
            if (Vector3.Distance(startPoint.position, endPoint.position) > distance && isActive == false)
            {
                OnPause.Invoke();
                StartCoroutine(ReturnPosRoutine());
            }
        }
        else if (isGrabOpen)
        {
            if(Vector3.Distance(startPointOpen.position, endPointOpen.position) > distance && isActiveOpen == false)
            {
                OffPause.Invoke();
                StartCoroutine(ReturnPosRoutineOpen());
            }
        }

    }

    IEnumerator ReturnPosRoutine()
    {
        if(isGrabOpen) isGrabOpen = false;
        while (true) 
        {
            
            grabObj.transform.position = Vector3.MoveTowards(grabObj.transform.position, finishPoint.position, 2 * Time.deltaTime);
            if (Vector3.Distance(grabObj.transform.position, finishPoint.position) <= 0.1f)
            {
                print("º¹±Í");
                isActive = true;
                isGrab = false;
                yield break;
            }
            yield return null;
        }
    }
    IEnumerator ReturnPosRoutineOpen()
    {
        if(isGrab) isGrab = false;
        while (true)
        {
            openGrabObj.transform.position = Vector3.MoveTowards(openGrabObj.transform.position, finishPointOpen.position, 2 * Time.deltaTime);
            if (Vector3.Distance(openGrabObj.transform.position, finishPointOpen.position) <= 0.1f)
            {
                print("º¹±ÍOpen");
                isActiveOpen = true;
                isGrabOpen = false;
                yield break;
            }
            yield return null;
        }
    }

    public void OnGrab()
    {
        print("1");
        if (photonView.IsMine == false)
        {
            photonView.RequestOwnership();
        }
        isGrab = true;
        isActive = false;
    }

    public void OnOpenGrab()
    {
        print("2");
        if (photonView.IsMine == false)
        {
            photonView.RequestOwnership();
        }
        isGrabOpen = true;
        isActiveOpen = false;
    }
}

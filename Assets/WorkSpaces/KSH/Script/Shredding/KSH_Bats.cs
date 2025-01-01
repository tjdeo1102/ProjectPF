using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Bats : MonoBehaviour
{
    [SerializeField] Collider bat;

    private void Start()
    {
        bat.enabled = false;
    }

    public void OnSelecGrab()
    {
        bat.enabled = true;
    }

    public void OnSelecExitGrab()
    {
        bat.enabled = false;
    }
}

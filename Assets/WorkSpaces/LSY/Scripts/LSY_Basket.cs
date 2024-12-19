using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_Basket : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Collider>() == null)
        {

        }
    }
}

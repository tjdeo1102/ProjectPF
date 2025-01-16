using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_Plate : MonoBehaviour
{
    [SerializeField] public bool IsFlower = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            IsFlower = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            IsFlower = false;
        }
    }
}

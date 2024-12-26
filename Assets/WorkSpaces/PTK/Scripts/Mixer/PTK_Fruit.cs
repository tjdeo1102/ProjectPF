using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Fruit : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Fruit");

            Destroy(collision.gameObject);

            Destroy(gameObject);
        }
    }
}

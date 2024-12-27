using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_XRBody : MonoBehaviour
{
    [SerializeField] private Transform HeadCamera;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = HeadCamera.position + offset;
        transform.rotation = Quaternion.Euler(transform.rotation.x, HeadCamera.rotation.y, transform.rotation.z);
    }
}

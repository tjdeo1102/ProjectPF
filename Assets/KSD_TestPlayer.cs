using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSD_TestPlayer : MonoBehaviour
{
    [SerializeField] Transform transform;
    [SerializeField] Vector3 position;

    private void Start()
    {
        transform.localPosition = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_WGH_PerfumeMaterialType
{
    None = 0,
    HOT,
    COOL,
    COMFORTABLE,
    SPICY,
    E_WGH_PERFUMEMATERIAL_MAX
}
public class WGH_TestPerfumeMaterial : MonoBehaviour
{
    public E_WGH_PerfumeMaterialType PerfumeMaterialType;
}

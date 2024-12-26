using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeMaterialName
{
    Cherry, Blueberry, Lemon, Grapefruit, Cosmos, Neroli,
    Galbanum, TreeBark, GreenTea, Rose , SIze
}

[Serializable]
public enum PerfumeMaterialType
{
    Small, Big, Hub, Size
}

[Serializable]
public enum PerfumeMaterialState
{
    Raw, Process, Size
}


// 재료의 정보를 가진 클래스
[Serializable]
public class KSD_PerfumeMaterialInfo
{
    [Header("기본 설정")]
    public PerfumeMaterialName Name;
    public PerfumeMaterialType Type;
    public PerfumeMaterialState State;

    [Header("현재 상태")]
    public int MaterialCount = 0;
}

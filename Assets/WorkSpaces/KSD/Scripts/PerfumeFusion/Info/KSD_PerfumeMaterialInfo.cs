using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeMaterialName
{
    Cherry, Blueberry, Lemon, Grapefruit, Cosmos, Neroli,
    Galbanum, TreeBark, GreenTea, Rose , Null
}

[Serializable]
public enum PerfumeMaterialType
{
    Small, Big, Hub, Null
}

[Serializable]
public enum PerfumeMaterialState
{
    Raw, Process, Null
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
    public float MaterialAmount = 0;

    public Color GetColorByName(PerfumeMaterialName name)
    {
        switch (name)
        {
            case PerfumeMaterialName.Cherry: return Color.red;
            case PerfumeMaterialName.Blueberry: return new Color(0.3f, 0.3f, 1f); // 블루 계열
            case PerfumeMaterialName.Lemon: return Color.yellow;
            case PerfumeMaterialName.Grapefruit: return new Color(1f, 0.5f, 0.3f); // 오렌지 계열
            case PerfumeMaterialName.Cosmos: return new Color(1f, 0.75f, 0.8f); // 핑크 계열
            case PerfumeMaterialName.Neroli: return new Color(0.9f, 0.9f, 0.5f); // 연노랑
            case PerfumeMaterialName.Galbanum: return Color.green;
            case PerfumeMaterialName.TreeBark: return new Color(0.4f, 0.25f, 0.1f); // 브라운 계열
            case PerfumeMaterialName.GreenTea: return new Color(0.6f, 0.9f, 0.6f); // 연녹색
            case PerfumeMaterialName.Rose: return new Color(1f, 0.4f, 0.6f); // 장미색
            default: return Color.white; // 기본값
        }
    }
}

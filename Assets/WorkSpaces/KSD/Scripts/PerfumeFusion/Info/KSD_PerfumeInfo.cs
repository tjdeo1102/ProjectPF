using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeName
{
    Null, Apricot, Aqua, Blue, Green, LightNavy, Pink, Purple, Red, Yellow
}

[Serializable]
public class KSD_PerfumeInfo
{
    [Header("기본 설정")]
    public PerfumeName Name;

    public Color GetColorByName(PerfumeName name)
    {
        switch (name)
        {
            case PerfumeName.Apricot: return new Color(1f, 0.8f, 0.6f); // 살구색
            case PerfumeName.Aqua: return new Color(0.5f, 1f, 1f); // 아쿠아 계열
            case PerfumeName.Blue: return Color.blue;
            case PerfumeName.Green: return Color.green;
            case PerfumeName.LightNavy: return new Color(0.2f, 0.4f, 0.8f); // 연한 네이비
            case PerfumeName.Pink: return new Color(1f, 0.6f, 0.8f); // 핑크 계열
            case PerfumeName.Purple: return new Color(0.6f, 0.4f, 1f); // 보라 계열
            case PerfumeName.Red: return Color.red;
            case PerfumeName.Yellow: return Color.yellow;
            default: return Color.white; // 기본값
        }
    }
}
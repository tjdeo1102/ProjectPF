using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeName
{
    Apricot, Aqua, Blue, Green, LightNavy, Pink, Purple, Red, Yellow, SIze
}

// 재료의 정보를 가진 클래스
[Serializable]
public class KSD_PerfumeInfo
{
    [Header("기본 설정")]
    public PerfumeName Name;

    [Header("현재 상태")]
    public int PerfumeCount = 0;
    public bool isPrivate = false;      // 레시피 공개 여부

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeNoteName
{
    Floral, Fruity, Citrus, Woody, Green, Alcohol, Null
}

[Serializable]
public enum PerfumeNoteState
{
    Concentrate, Note, Null
}

[Serializable]
public class KSD_PerfumeNoteInfo
{
    [Header("기본 설정")]
    public PerfumeNoteName Name;
    public PerfumeNoteState State = PerfumeNoteState.Concentrate;

    [Header("현재 상태")]
    public int NoteCount = 0;

    public Color GetColorByName(PerfumeNoteName name)
    {
        switch (name)
        {
            case PerfumeNoteName.Floral: return new Color(1f, 0.5f, 0.8f); // 핑크 계열
            case PerfumeNoteName.Fruity: return new Color(1f, 0.7f, 0.2f); // 오렌지 계열
            case PerfumeNoteName.Citrus: return Color.yellow;
            case PerfumeNoteName.Woody: return new Color(0.6f, 0.4f, 0.2f); // 브라운 계열
            case PerfumeNoteName.Green: return Color.green;
            case PerfumeNoteName.Alcohol: return new Color(0.8f, 0.8f, 0.8f); // 회색 계열
            default: return Color.white; // 기본값
        }
    }
}
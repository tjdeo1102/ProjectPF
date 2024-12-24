using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PerfumeNoteName
{
    Floral, Fruity, Citrus, Woody, Green , Alcohol , SIze
}

[Serializable]
public enum PerfumeNoteState
{
    Concentrate, Note, SIze
}

// 재료의 정보를 가진 클래스
[Serializable]
public class KSD_PerfumeNoteInfo
{
    [Header("기본 설정")]
    public PerfumeNoteName Name;
    public PerfumeNoteState State = PerfumeNoteState.Concentrate;

    [Header("현재 해당 노트 수량")]
    public int NoteCount = 0;
}

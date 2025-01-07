using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LSY_BucketInfo
{
    public PerfumeNoteName noteName;
    public ParticleSystem bucketParticle;
    public Color liquidColor;
}

[Serializable]
public class LSY_DispensorInfo
{
    public PerfumeNoteName noteName;
    public PerfumeNoteState state;
    public Color liquidColor;
    public Color liquidLineColor;
}

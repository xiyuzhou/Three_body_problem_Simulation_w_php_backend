using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class DataObject
{
    public string name;
    public float GravitationalConst;
    public float TimeStep;
    public float Size;
    public float duration;
    public float[] star1Info = new float[7];
    public float[] star2Info = new float[7];
    public float[] star3Info = new float[7];
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTargetContainer : MonoBehaviour
{
    public Rigidbody rigid;
    public Transform targetPos;
    public bool onTarget;
    public bool inRange;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        targetPos = GetComponent<Transform>();
        onTarget = true;
        inRange = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public List<Transform> targets;
    public float distanceThreshold = 50;
    public float smoothTime = 0.5f;
    public Camera cam;
    public Transform camRotation;
    public float CameraWeight;
    public int switchCamState = 0; //(zero means auto)


    private List<Transform> camTargets = new List<Transform>();
    private Transform maxMassObj;
    private float size;
    private Vector3 velocity;
    private Vector3 centerMass;
    private Vector3 camTargetsMass;
    private void Start()
    {
        var max = targets[1];
        for (int i = 1; i < targets.Count;i++)
        {
            if (targets[i].gameObject.GetComponent<Rigidbody>().mass > max.gameObject.GetComponent<Rigidbody>().mass)
            {
                max = targets[i];
            }
        }
        maxMassObj = max;
        CalculateCenterOfMass();
        camTargetsMass = centerMass;
    }
    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;
        checkWithinRange();
        Move();
        zoom();
    }

    void checkWithinRange()
    {
        if (camTargets!=null)
            camTargets.Clear();
        foreach (Transform t in targets)
        {
            if(Vector3.Distance(camTargetsMass, t.position) < distanceThreshold)
            {
                camTargets.Add(t);
            }
        }
        if (camTargets.Count == 0)
            camTargets.Add(maxMassObj);
        CalculateCenterOfMass();
    }
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPos = CameraWeight * centerPoint + (1- CameraWeight) * camTargetsMass;
        camRotation.position = Vector3.SmoothDamp(camRotation.position, newPos, ref velocity, smoothTime);
    }
    void zoom()
    {
        float newZoom = Mathf.Lerp(30, 100, size/100);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    Vector3 GetCenterPoint()
    {

        if (camTargets.Count == 1)
        {
            size = 0;
            return camTargets[0].position;
        }
        var bounds = new Bounds(camTargets[0].position, Vector3.zero);
        
        for (int i = 0; i < camTargets.Count; i++)
        {
            bounds.Encapsulate(camTargets[i].position);
        }
        size = bounds.size.x + bounds.size.y + bounds.size.z;
        return bounds.center;
    }

    void CalculateCenterOfMass()
    {
        Vector3 sumOfPositions = Vector3.zero;
        float totalMass = 0f;

        Vector3 sumCamTarPos = Vector3.zero;
        float totalCamMass = 0f;

        foreach (Transform target in targets)
        {
            Rigidbody rigid = target.gameObject.GetComponent<Rigidbody>();
            sumOfPositions += target.position * rigid.mass;
            totalMass += rigid.mass;
            if (camTargets.Contains(target)){
                sumCamTarPos += target.position * rigid.mass;
                totalCamMass += rigid.mass;
            }
        }

        camTargetsMass = sumCamTarPos / totalCamMass;

        centerMass = sumOfPositions / totalMass;
    }

}


using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> targets;
    public List<TargetContainer> targetsInfo;
    public int switchCamState = 0; //(zero means auto)
    public float distanceThreshold = 50;
    public float smoothTime = 0.5f;
    public float cameraWeight = 0.5f;
    public Transform camParent;
    public Camera cam;
    private TargetContainer focusObject;
    private Vector3 velocity;
    private Vector3 refOffsetSpeed;
    private Vector3 centerMass;
    private float size;
    public static CameraController instance;
    public float mouseSensitivity = 3.5f;

    private Vector3 CamOffset;
    private void Start()
    {
        instance = this;
        OnStart();
    } 
    public void OnStart()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("No targets assigned to CameraController.");
            return;
        }

        targetsInfo = new List<TargetContainer>();

        foreach (GameObject target in targets)
        {
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                TargetContainer info = new TargetContainer();
                info.transform = target.transform;
                info.mass = targetRigidbody.mass;
                info.inRange = true;
                targetsInfo.Add(info);
            }
            else
            {
                Debug.LogError("Target " + target.name + " does not have a Rigidbody.");
            }
        }

        focusObject = FindMaxMassTarget();
    }

    private void LateUpdate()
    {
        CalculateCenterOfMass();
        CheckWithinRange();
        Move();
        Zoom();
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = -Input.GetAxis("Mouse Y");
            camParent.RotateAround(camParent.position, camParent.up, x * mouseSensitivity);

            camParent.RotateAround(camParent.position, camParent.right, y * mouseSensitivity);

        }
    }

    private void Move()
    {
        Vector3 newPos = new Vector3();
        if (switchCamState == 0)
        {
            Vector3 centerPoint = GetCenterPoint();
            newPos = cameraWeight * centerPoint + (1 - cameraWeight) * centerMass;
        }
        else
        {
            newPos = targetsInfo[switchCamState-1].transform.position;
        }
        camParent.position = Vector3.SmoothDamp(camParent.position, newPos, ref velocity, smoothTime);
        cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, CamOffset, ref refOffsetSpeed, 0.5f);
    }

    private void Zoom()
    {
        float newZoom = 0;
        if (switchCamState == 0)
            newZoom = Mathf.Lerp(30, 80, size / 100);
        else
            newZoom = 35;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(focusObject.transform.position, Vector3.zero);
        foreach (TargetContainer tar in targetsInfo)
        {
            if (tar.inRange)
            {
                bounds.Encapsulate(tar.transform.position);
            }
        }
        size = bounds.size.x + bounds.size.y + bounds.size.z;
        return bounds.center;
    }

    private TargetContainer FindMaxMassTarget()
    {
        TargetContainer maxMassTarget = null;
        float maxMass = 0;
        foreach (TargetContainer targetInfo in targetsInfo)
        {
            if (targetInfo.inRange && targetInfo.mass > maxMass)
            {
                maxMass = targetInfo.mass;
                maxMassTarget = targetInfo;
            }
        }
        return maxMassTarget;
    }

    public void SetCamState(int state)
    {
        switchCamState = state;
    }

    private void CheckWithinRange()
    {
        bool allIn = false;
        foreach (TargetContainer tar in targetsInfo)
        {
            tar.inRange = Vector3.Distance(centerMass, tar.transform.position) < distanceThreshold;
            allIn = tar.inRange;
        }
        if (!allIn)
        {
            focusObject.inRange = true;
        }

    }

    private void CalculateCenterOfMass()
    {
        Vector3 sumOfPositions = Vector3.zero;
        float totalMass = 0f;

        foreach (TargetContainer tar in targetsInfo)
        {
            if (tar.inRange)
            {
                sumOfPositions += tar.transform.position * tar.mass;
                totalMass += tar.mass;
            }
        }

        if (totalMass > 0f)
        {
            centerMass = sumOfPositions / totalMass;
        }
        else
        {
            Debug.LogWarning("Total mass of targets within range is zero. Center of mass calculation skipped.");
        }
    }
    public void OffsetCamera(Vector3 offset)
    {
        CamOffset = offset;
    }
}

[Serializable]
public class TargetContainer
{
    public Transform transform;
    public float mass;
    public bool inRange;
}

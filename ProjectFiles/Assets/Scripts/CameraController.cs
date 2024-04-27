using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public StarTargetContainer[] targets;
    public int switchCamState = 0; //(zero means auto)
    public float distanceThreshold = 50;
    public float smoothTime = 0.5f;
    public float cameraWeight = 0.5f;
    public Transform camParent;
    public Camera cam;
    private StarTargetContainer focusObject;
    private Vector3 velocity;
    private Vector3 refOffsetSpeed;
    private Vector3 centerMass;
    private float size;
    public static CameraController instance;
    public float mouseSensitivity = 3.5f;

    private float CamX = 0;
    private float CamY = 0;
    private float refX;
    private float refY;
    private Vector3 CamOffset;
    public Slider camOffest;
    private void Start()
    {
        instance = this;
        OnStart();
    } 
    public void OnStart()
    {
        FindMaxMassTarget();
    }
    private void LateUpdate()
    {
        CalculateCenterOfMass();
        CheckWithinRange();
        Move();
        Zoom();
        handleCameraRotation();
    }
    private void handleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            CamX = (CamX + x) / 2;
            float y = -Input.GetAxis("Mouse Y");
            CamY = (CamY + y) / 2;
            camParent.RotateAround(camParent.position, camParent.up, x * mouseSensitivity);
            camParent.RotateAround(camParent.position, camParent.right, y * mouseSensitivity);
        }
        else
        {
            if (CamX != 0)
            {
                CamX = Mathf.SmoothDamp(CamX, 0, ref refX, 0.5f);
                camParent.RotateAround(camParent.position, camParent.up, CamX * mouseSensitivity);
            }
            if (CamY != 0)
            {
                CamY = Mathf.SmoothDamp(CamY, 0, ref refY, 0.5f);
                camParent.RotateAround(camParent.position, camParent.right, CamY * mouseSensitivity);
            }
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
            newPos = targets[switchCamState-1].transform.position;
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
        var firstValidTarget = targets.FirstOrDefault(tar => tar.inRange && tar.onTarget);
        var bounds = new Bounds(firstValidTarget.transform.position, Vector3.zero);
        foreach (StarTargetContainer tar in targets)
        {
            if (tar.inRange && tar.onTarget)
            {
                bounds.Encapsulate(tar.transform.position);
            }
        }
        size = bounds.size.x + bounds.size.y + bounds.size.z;
        return bounds.center;
    }

    public void FindMaxMassTarget()
    {
        float maxMass = 0;
        foreach (StarTargetContainer targetInfo in targets)
        {
            if (targetInfo.rigid.mass > maxMass)
            {
                maxMass = targetInfo.rigid.mass;
                focusObject = targetInfo;
            }
        }
    }

    public void SetCamState(int state)
    {
        switchCamState = state;
    }

    private void CheckWithinRange()
    {
        bool allIn = false;
        foreach (StarTargetContainer tar in targets)
        {
            if (!tar.onTarget)
            {
                allIn = false;
                tar.inRange = false;
                continue;
            }
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

        foreach (StarTargetContainer tar in targets)
        {
            if (tar.inRange && tar.onTarget)
            {
                sumOfPositions += tar.transform.position * tar.rigid.mass;
                totalMass += tar.rigid.mass;
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

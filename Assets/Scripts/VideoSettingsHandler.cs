using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider CamWeight;
    [SerializeField] private Slider CamSmooth;
    [SerializeField] private Slider CamDistance;
    [SerializeField] private TMP_InputField distanceMaxValue;
    [SerializeField] private TMP_InputField distanceThreshold;

    private void Start()
    {
        SetCamWeight();
        SetCamSmoothTime();
        SetDistanceThreshold();
        SetDistanceMaxValue();
        SetCameraOffest();
    }
    public void SetCameraMode(int i)
    {
        CameraController.instance.SetCamState(i);
    }
    public void SetCamWeight()
    {
        CameraController.instance.cameraWeight = CamWeight.value;
    }
    public void SetCamSmoothTime()
    {
        CameraController.instance.smoothTime = CamSmooth.value;
    }
    public void SetDistanceThreshold()
    {
        CameraController.instance.distanceThreshold = int.Parse(distanceThreshold.text);
    }
    public void SetDistanceMaxValue()
    {
        int value = int.Parse(distanceMaxValue.text);
        if (CamDistance.value > value)
            CamDistance.value = value;
        CamDistance.maxValue = value;
    }
    public void SetCameraOffest()
    {
        Vector3 offset = new Vector3(0, 0, -CamDistance.value);
        Debug.Log(offset);
        CameraController.instance.OffsetCamera(offset);
    }
}

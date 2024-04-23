using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsHandler : MonoBehaviour
{
    [SerializeField] private Slider CamWeight;
    [SerializeField] private Slider CamSmooth;
    [SerializeField] private TMP_InputField distanceThreshold;

    private void Start()
    {
        SetCamWeight();
        SetCamSmoothTime();
        SetDistanceThreshold();
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
}

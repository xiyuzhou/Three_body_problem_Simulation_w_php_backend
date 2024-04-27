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
    [SerializeField] private TextMeshProUGUI timeStepText;
    public void SetTimeStep(bool k)
    {
        float i = k ? 0.2f : -0.2f;
        float j = Time.timeScale + (k ? 0.2f : -0.2f);
        j = Mathf.Clamp(j, 0, 3);
        Time.timeScale = j;
        timeStepText.text = Time.timeScale.ToString("F1");
    }
    private void Start()
    {
        SetCamWeight();
        SetCamSmoothTime();
        SetDistanceThreshold();
        SetDistanceMaxValue();
        SetCameraOffest();
    }
    private void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            CamDistance.value -=2;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            CamDistance.value +=2;
        }
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
        CameraController.instance.OffsetCamera(offset);
    }
}

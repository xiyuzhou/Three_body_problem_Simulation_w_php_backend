using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Slider slider;
    public void OnChange()
    {
        text.text = slider.value.ToString("F1");
    }
    private void Awake()
    {
        OnChange();
    }

}

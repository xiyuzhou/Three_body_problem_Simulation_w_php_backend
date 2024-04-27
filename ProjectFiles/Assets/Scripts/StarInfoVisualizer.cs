using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StarInfoVisualizer : MonoBehaviour
{
    public Rigidbody[] bodies;
    public TextMeshProUGUI[] speed;
    public TextMeshProUGUI[] energy;

    public TextMeshProUGUI[] TotalEnergy;
    private float SystemTotalEnergy;
    [SerializeField]SimulationController simulationController;
    private void FixedUpdate()
    {
        SystemTotalEnergy = 0;
        for (int i = 0; i < bodies.Length; i++)
        {
            float magnitude = bodies[i].velocity.magnitude;
            speed[i].text = magnitude.ToString("F3");
            float Kinetic = 0.5f * bodies[i].mass * bodies[i].velocity.sqrMagnitude;
            energy[i].text = Kinetic.ToString("F3");
            SystemTotalEnergy += Kinetic;
        }
        TotalEnergy[0].text = SystemTotalEnergy.ToString("F3");
        float g = simulationController.gravitationalPotential;
        TotalEnergy[1].text = (g == -1? "infinity" : g.ToString("F3"));
        TotalEnergy[2].text = (SystemTotalEnergy - g).ToString();
    }

}

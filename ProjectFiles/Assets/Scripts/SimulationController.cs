using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimulationController : MonoBehaviour
{
    public GameObject[] starObjects;
    private Rigidbody[] bodies = new Rigidbody[3];
    public float gravitationalConstant = 6.67430e-11f; // Gravitational constant (m^3/kg/s^2)
    [SerializeField] private CustomDataHandler customDataHandler;
    public bool running = false;
    public void OnStartSimulation()
    {
        DataObject data = customDataHandler.GetCurrentData();

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].isKinematic = false;
            var trail = starObjects[i].GetComponent<TrailRenderer>();
            trail.Clear();
            trail.time = data.duration;
            trail.enabled = true;
        }

        var star1 = data.star1Info;
        bodies[0].mass = star1[0];
        bodies[0].position = new Vector3(star1[1], star1[2], star1[3]);
        bodies[0].velocity = new Vector3(star1[4], star1[5], star1[6]);
        var star2 = data.star2Info;
        bodies[1].mass = star2[0];
        bodies[1].position = new Vector3(star2[1], star2[2], star2[3]);
        bodies[1].velocity = new Vector3(star2[4], star2[5], star2[6]);
        var star3 = data.star3Info;
        bodies[2].mass = star3[0];
        bodies[2].position = new Vector3(star3[1], star3[2], star3[3]);
        bodies[2].velocity = new Vector3(star3[4], star3[5], star3[6]);

        for (int i = 0; i < starObjects.Length; i++)
        {
            float a = bodies[i].mass > 1 ? Mathf.Pow(bodies[i].mass, 1f / 3f) : 1;
            a = a * data.Size;
            starObjects[i].transform.localScale = new Vector3(a, a, a);
        }
        CameraController.instance.OnStart();

        gravitationalConstant = data.GravitationalConst;
        running = true;
    }
    public void OnResetSimulation()
    {
        for (int i = 0; i < starObjects.Length; i++)
        {
            bodies[i].isKinematic = true;
            starObjects[i].GetComponent<TrailRenderer>().Clear();
            starObjects[i].GetComponent<TrailRenderer>().enabled = false;
        }
        setDataDefault();
        running = false;
    }
    public void onDataValueChange()
    {
        if (!running)
        {
            setDataDefault();
        }
    }
    private void setDataDefault()
    {
        DataObject data = customDataHandler.GetCurrentData();
        bodies[0].mass = data.star1Info[0];
        starObjects[0].transform.position = new Vector3(data.star1Info[1], data.star1Info[2], data.star1Info[3]);
        bodies[1].mass = data.star2Info[0];
        starObjects[1].transform.position = new Vector3(data.star2Info[1], data.star2Info[2], data.star2Info[3]);
        bodies[2].mass = data.star3Info[0];
        starObjects[2].transform.position = new Vector3(data.star3Info[1], data.star3Info[2], data.star3Info[3]);

        for (int i = 0; i < starObjects.Length; i++)
        {
            float a = bodies[i].mass > 1 ? Mathf.Pow(bodies[i].mass, 1f / 3f) : 1;
            starObjects[i].transform.localScale = new Vector3(a, a, a);
        }

    }
    private void Start()
    {
        for (int i = 0; i < starObjects.Length; i++)
        {
            bodies[i] = starObjects[i].GetComponent<Rigidbody>();
        }
        OnResetSimulation();
        //OnStartSimulation();
        //CameraController.instance.OnStart();
    }
    void FixedUpdate()
    {
        if (running)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                for (int j = i + 1; j < bodies.Length; j++)
                {
                    // Calculate the vector from body i to body j
                    Vector3 direction = bodies[j].position - bodies[i].position;
                    float distance = direction.magnitude;
                    direction.Normalize();

                    // Calculate the gravitational force between the bodies
                    if (distance > 0)
                    {
                        float forceMagnitude = gravitationalConstant * (bodies[i].mass * bodies[j].mass) / Mathf.Pow(distance, 2);
                        Vector3 force = direction * forceMagnitude;

                        // Apply the gravitational force to each body
                        bodies[i].AddForce(force);
                        bodies[j].AddForce(-force);
                    }
                }
            }
        }
        // Loop through each pair of bodies
    }
}

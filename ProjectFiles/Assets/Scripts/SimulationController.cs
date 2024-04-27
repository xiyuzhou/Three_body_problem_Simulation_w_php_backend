using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public GameObject[] starObjects;
    private List<Rigidbody> bodies = new List<Rigidbody>();
    public float gravitationalConstant = 6.67430e-11f; // Gravitational constant (m^3/kg/s^2)
    [SerializeField] private CustomDataHandler customDataHandler;
    public bool running = false;
    public float gravitationalPotential = 0;
    public float collisionFactor;
    public float[] lastPotential = new float[3];
    public Vector3 totalMomentum = Vector3.zero;
    public void OnStartSimulation()
    {
        OnResetSimulation();
        DataObject data = customDataHandler.GetCurrentData();
        foreach (var item in bodies)
        {
            item.isKinematic = false;
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

            var trail = starObjects[i].GetComponent<TrailRenderer>();
            trail.emitting = true;
            trail.Clear();
            trail.time = data.duration * Mathf.Sqrt(a);
            trail.widthMultiplier = a / 4;
        }
        CameraController.instance.OnStart();

        gravitationalConstant = data.GravitationalConst;
        running = true;
        foreach (var b in bodies)
        {
            totalMomentum += b.mass * b.velocity;
        }
    }
    public void OnResetSimulation()
    {
        bodies.Clear();
        for (int i = 0; i < starObjects.Length; i++)
        {
            bodies.Add(starObjects[i].GetComponent<Rigidbody>());
            starObjects[i].SetActive(true);
        }

        for (int i = 0; i < starObjects.Length; i++)
        {
            bodies[i].isKinematic = true;
            starObjects[i].GetComponent<TrailRenderer>().Clear();
            starObjects[i].GetComponent<TrailRenderer>().emitting = false;
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
            a = a * data.Size;
            starObjects[i].transform.localScale = new Vector3(a, a, a);
        }

    }
    private void Start()
    {
        OnResetSimulation();
    }
    void FixedUpdate()
    {
        if (running)
        {
            //float softeningFactor = 0.2f;
            gravitationalPotential = 0;
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {

                    // Calculate the vector from body i to body j
                    Vector3 direction = bodies[j].position - bodies[i].position;
                    float distance = direction.magnitude;
                    direction.Normalize();

                    if (distance > 0)
                    {
                        //distance = Mathf.Max(distance, softeningFactor);
                        if (distance < 0.1f)// handle collision
                        {
                            bodies[j].gameObject.SetActive(false);
                            float newMass = bodies[j].mass + bodies[i].mass;
                            float a = Mathf.Pow(newMass / bodies[i].mass, 1f / 3f);
                            Vector3 newVelocity = (bodies[i].velocity * bodies[i].mass + bodies[j].velocity * bodies[j].mass)/newMass;
                            bodies[i].velocity = newVelocity;
                            bodies[i].mass = newMass;
                            
                            starObjects[i].transform.localScale *= a;

                            bodies.RemoveAt(j);
                            Debug.Log("Collided");
                            
                            continue;
                        }
                        float energy = gravitationalConstant * (bodies[i].mass * bodies[j].mass) / distance;
                        float forceMagnitude = energy / distance;
                        gravitationalPotential += energy;
                        Vector3 force = direction * forceMagnitude;

                        bodies[i].AddForce(force);
                        bodies[j].AddForce(-force);
                    }
                }
            }
        }
        /*
        void Update()
        {
            if (running)
            {
                for (int i = 0; i < bodies.Length; i++)
                {
                    for (int j = i + 1; j < bodies.Length; j++)
                    {
                        Vector3 direction = bodies[j].position - bodies[i].position;
                        float distance = direction.magnitude;
                        direction.Normalize();


                    }
                } 
                bodies[0].velocity += Time.deltaTime * gravitationalConstant * ((bodies[1].position - bodies[0].position).normalized * bodies[1].mass / Mathf.Pow(Vector3.Distance(bodies[0].position, bodies[1].position), 2) +
                                                            (bodies[2].position - bodies[0].position).normalized * bodies[2].mass / Mathf.Pow(Vector3.Distance(bodies[0].position, bodies[2].position), 2));

                bodies[1].velocity += Time.deltaTime * gravitationalConstant * ((bodies[0].position - bodies[1].position).normalized * bodies[0].mass / Mathf.Pow(Vector3.Distance(bodies[1].position, bodies[0].position), 2) +
                                                                (bodies[2].position - bodies[1].position).normalized * bodies[2].mass / Mathf.Pow(Vector3.Distance(bodies[1].position, bodies[2].position), 2));

                bodies[2].velocity += Time.deltaTime * gravitationalConstant * ((bodies[0].position - bodies[2].position).normalized * bodies[0].mass / Mathf.Pow(Vector3.Distance(bodies[2].position, bodies[0].position), 2) +
                                                                (bodies[1].position - bodies[2].position).normalized * bodies[1].mass / Mathf.Pow(Vector3.Distance(bodies[2].position, bodies[1].position), 2));

            }
        }*/
        /*
        void FixedUpdate()
        {
            if (running)
            {
                int k = 0;
                gravitationalPotential = 0;
                float softeningFactor = 0.1f; // Adjust this value as needed

                for (int i = 0; i < bodies.Length; i++)
                {
                    for (int j = i + 1; j < bodies.Length; j++)
                    {

                        // Calculate the vector from body i to body j
                        Vector3 direction = bodies[j].position - bodies[i].position;
                        Vector3 positionDiff = (bodies[j].position + bodies[j].velocity * Time.fixedDeltaTime) - (bodies[i].position + bodies[i].velocity * Time.fixedDeltaTime);
                        float newDistance = positionDiff.magnitude;
                        float distance = direction.magnitude;
                        direction.Normalize();

                        if (distance > 0)
                        {

                            float energy = gravitationalConstant * (bodies[i].mass * bodies[j].mass) / distance;

                            float energydiff = gravitationalConstant * (bodies[i].mass * bodies[j].mass) / newDistance-energy;
                            float forceMagnitude = energy / distance;
                            gravitationalPotential += energy;
                            Vector3 force = direction * forceMagnitude;

                            float a = Vector3.Dot(force, bodies[i].velocity * Time.fixedDeltaTime);
                            float b = Vector3.Dot(-force, bodies[j].velocity * Time.fixedDeltaTime);

                            float scaleFactor = (a + b) / energydiff;
                            bodies[i].velocity /= scaleFactor;
                            bodies[j].velocity /= scaleFactor;

                            lastPotential[k] = energy;
                            bodies[i].AddForce(force);
                            bodies[j].AddForce(-force);

                        }
                        k++;
                    }
                }
            }
        }*/
    }
}
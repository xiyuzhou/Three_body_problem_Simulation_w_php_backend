using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public StarTargetContainer[] starObjects;
    //private List<Rigidbody> bodies = new List<Rigidbody>();
    public float gravitationalConstant = 6.67430e-11f; // Gravitational constant (m^3/kg/s^2)
    [SerializeField] private CustomDataHandler customDataHandler;
    public bool running = false;
    public float gravitationalPotential = 0;
    public float collisionFactor;
    public float[] lastPotential = new float[3];
    public void OnStartSimulation()
    {
        OnResetSimulation();
        DataObject data = customDataHandler.GetCurrentData();
        foreach (var item in starObjects)
        {
            item.rigid.isKinematic = false;
        }
        var star1 = data.star1Info;
        starObjects[0].rigid.mass = star1[0];
        starObjects[0].rigid.position = new Vector3(star1[1], star1[2], star1[3]);
        starObjects[0].rigid.velocity = new Vector3(star1[4], star1[5], star1[6]);
        var star2 = data.star2Info;
        starObjects[1].rigid.mass = star2[0];
        starObjects[1].rigid.position = new Vector3(star2[1], star2[2], star2[3]);
        starObjects[1].rigid.velocity = new Vector3(star2[4], star2[5], star2[6]);
        var star3 = data.star3Info;
        starObjects[2].rigid.mass = star3[0];
        starObjects[2].rigid.position = new Vector3(star3[1], star3[2], star3[3]);
        starObjects[2].rigid.velocity = new Vector3(star3[4], star3[5], star3[6]);

        for (int i = 0; i < starObjects.Length; i++)
        {
            float a = starObjects[i].rigid.mass > 1 ? Mathf.Pow(starObjects[i].rigid.mass, 1f / 3f) : 1;
            a = a * data.Size;
            starObjects[i].targetPos.localScale = new Vector3(a, a, a);

            var trail = starObjects[i].GetComponent<TrailRenderer>();
            trail.emitting = true;
            trail.Clear();
            trail.time = data.duration * Mathf.Sqrt(a);
            trail.widthMultiplier = a / 4;
        }
        CameraController.instance.OnStart();

        gravitationalConstant = data.GravitationalConst;
        running = true;
    }
    public void OnResetSimulation()
    {
        foreach (var obj in starObjects)
        {
            obj.gameObject.SetActive(true);
            obj.inRange = true;
            obj.onTarget = true;
            obj.rigid.isKinematic = true;
            obj.gameObject.GetComponent<TrailRenderer>().Clear();
            obj.gameObject.GetComponent<TrailRenderer>().emitting = false;
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
        starObjects[0].rigid.mass = data.star1Info[0];
        starObjects[0].transform.position = new Vector3(data.star1Info[1], data.star1Info[2], data.star1Info[3]);
        starObjects[1].rigid.mass = data.star2Info[0];
        starObjects[1].transform.position = new Vector3(data.star2Info[1], data.star2Info[2], data.star2Info[3]);
        starObjects[2].rigid.mass = data.star3Info[0];
        starObjects[2].transform.position = new Vector3(data.star3Info[1], data.star3Info[2], data.star3Info[3]);

        for (int i = 0; i < starObjects.Length; i++)
        {
            float a = starObjects[i].rigid.mass > 1 ? Mathf.Pow(starObjects[i].rigid.mass, 1f / 3f) : 1;
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
            for (int i = 0; i < starObjects.Length; i++)
            {
                if (!starObjects[i].onTarget)
                    continue;
                for (int j = i + 1; j < starObjects.Length; j++)
                {
                    if (!starObjects[j].onTarget)
                        continue;
                    // Calculate the vector from body i to body j
                    Vector3 direction = starObjects[j].rigid.position - starObjects[i].rigid.position;
                    float distance = direction.magnitude;
                    direction.Normalize();

                    if (distance > 0)
                    {
                        //distance = Mathf.Max(distance, softeningFactor);
                        if (distance < 0.1f)// handle collision
                        {
                            starObjects[j].onTarget = false;
                            starObjects[j].gameObject.SetActive(false);
                            float newMass = starObjects[j].rigid.mass + starObjects[i].rigid.mass;
                            float a = Mathf.Pow(newMass / starObjects[i].rigid.mass, 1f / 3f);
                            Vector3 newVelocity = (starObjects[i].rigid.velocity * starObjects[i].rigid.mass + starObjects[j].rigid.velocity * starObjects[j].rigid.mass)/newMass;
                            starObjects[i].rigid.velocity = newVelocity;
                            starObjects[i].rigid.mass = newMass;
                            
                            starObjects[i].transform.localScale *= a;
                            Debug.Log("Collided");
                            CameraController.instance.FindMaxMassTarget();
                            continue;
                        }
                        float energy = gravitationalConstant * (starObjects[i].rigid.mass * starObjects[j].rigid.mass) / distance;
                        float forceMagnitude = energy / distance;
                        gravitationalPotential += energy;
                        Vector3 force = direction * forceMagnitude;

                        starObjects[i].rigid.AddForce(force);
                        starObjects[j].rigid.AddForce(-force);
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
using System;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2.5f;
    public float turnSpeed = 5f;
    public float detectionRadious = 3.0f;
    public float alignmentMultiplier = 2.0f;
    public float cohesionMultiplier = 2.0f;
    public float separationMultiplier = 2.0f;
    public float directionMultiplier = 2.0f;

    private Func<Boid, Vector3> Alignment;
    private Func<Boid, Vector3> Cohesion;
    private Func<Boid, Vector3> Separation;
    private Func<Boid, Vector3> Direction;

    public void Init(Func<Boid, Vector3> Alignment,
        Func<Boid, Vector3> Cohesion,
        Func<Boid, Vector3> Separation,
        Func<Boid, Vector3> Direction)
    {
        this.Alignment = Alignment;
        this.Cohesion = Cohesion;
        this.Separation = Separation;
        this.Direction = Direction;
    }

    private void Update()
    {
        Vector3 acs = ACS();
        transform.forward = Vector3.Lerp(transform.forward, acs, turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }


    public Vector3 ACS()
    {
        Vector3 ACS = Alignment(this) * alignmentMultiplier + Cohesion(this) * cohesionMultiplier +
                      Separation(this) * separationMultiplier + Direction(this) * directionMultiplier;
        return ACS.normalized;
    }
}
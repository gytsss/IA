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
        transform.up = Vector3.Lerp(transform.up, ACS(), turnSpeed * Time.deltaTime);
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public Vector3 ACS()
    {
        
        Vector3 alignmentForce = Alignment(this) * alignmentMultiplier;
        Vector3 cohesionForce = Cohesion(this) * cohesionMultiplier;
        Vector3 separationForce = Separation(this) * separationMultiplier;
        Vector3 directionForce = Direction(this);
        
        Vector3 ACS = alignmentForce + cohesionForce + separationForce + directionForce;
        return ACS.normalized;
    }
}
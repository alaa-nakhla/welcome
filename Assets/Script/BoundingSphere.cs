using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingSphere
{
    public Vector3 center;
    public float radius;

    public BoundingSphere(Vector3 center, float radius)
    {
       this.center = center;
        this.radius = radius;
    }

    public bool Intersects(BoundingSphere other)
    {
        float distance = Vector3.Distance(center, other.center);
        return distance <= radius + other.radius;
    }

}

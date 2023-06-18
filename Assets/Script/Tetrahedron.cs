using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector3 D;
    public Vector3 normal;
    public float distance;  
    public bool IsTriangle;

    public Tetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
        CalculateNormal();
        DetermineTriangle(); 
    }

    private void CalculateNormal()
    {
        // Calculate the normal of the tetrahedron using the cross product
        normal = Vector3.Cross(B - A, C - A).normalized;
    }
     private void CalculateDistance()
    {
        // Calculate the signed distance from the origin to the plane of the tetrahedron
        distance = Vector3.Dot(normal, A);
    }
     private void DetermineTriangle()
    {
        // Determine if the tetrahedron is actually a triangle
        IsTriangle = Vector3.Dot(normal, D) == distance;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GJKVf : MonoBehaviour
{
    public float detectionRadius = 2f;
    public float restitution = 0.8f;
    public LayerMask objectLayer;
    private List<GameObject> nearestObjects;
    private void Start()
    {
        nearestObjects = new List<GameObject>();
    }
    private void Update()
    {
        FindNearestObjects();
        CheckCollisions();
    }
    private void FindNearestObjects()
    {
        nearestObjects.Clear();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject && obj.layer == objectLayer)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance <= detectionRadius)
                {
                    nearestObjects.Add(obj);
                }
            }
        }
    }

    private void CheckCollisions()
    {
        for (int i = 0; i < nearestObjects.Count; i++)
        {
            GameObject object1 = gameObject;
            GameObject object2 = nearestObjects[i];
            if (AABBCollisionCheck(object1, object2))
            {
                List<Vector3> collisionPoints = GJKCollisionCheck(object1, object2);
                if (collisionPoints.Count > 0)
                {
                    ResolveCollision(object1, object2, collisionPoints);
                }
            }
        }
    }

    private bool AABBCollisionCheck(GameObject object1, GameObject object2)
    {
        Renderer renderer1 = object1.GetComponent<Renderer>();
        Renderer renderer2 = object2.GetComponent<Renderer>();

        if (renderer1 != null && renderer2 != null)
        {
            Vector3 min1 = renderer1.bounds.min;
            Vector3 max1 = renderer1.bounds.max;
            Vector3 min2 = renderer2.bounds.min;
            Vector3 max2 = renderer2.bounds.max;
            if (max1.x >= min2.x && min1.x <= max2.x &&
                max1.y >= min2.y && min1.y <= max2.y &&
                max1.z >= min2.z && min1.z <= max2.z)
            {
                return true;
            }
        }

        return false;
    }

    public List<Vector3> GJKCollisionCheck(GameObject objectA, GameObject objectB)
    {
        Mesh meshA = objectA.GetComponent<MeshFilter>().sharedMesh;
        Mesh meshB = objectB.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] verticesA = meshA.vertices;
        Vector3[] verticesB = meshB.vertices;
        Vector3 direction = Vector3.one;
        Vector3[] simplex = new Vector3[3];
        int simplexSize = 0;
        simplex[simplexSize++] = Support(verticesA, verticesB, direction);
        direction = -simplex[0];
        int maxIterations = 100;
        int iterationCount = 0;
        while (iterationCount < maxIterations)
        {

            if (simplexSize >= simplex.Length)
            {
                Array.Resize(ref simplex, simplex.Length + 1);
            }
            simplex[simplexSize++] = Support(verticesA, verticesB, direction);
            if (Vector3.Dot(simplex[simplexSize - 1], direction) < 0)
            {

                return new List<Vector3>();
            }
            if (ContainsOrigin(simplex, ref simplexSize, ref direction))
            {

                List<Vector3> collisionPoints = ExtractCollisionPoints(simplex, simplexSize);
                return collisionPoints;
            }

            iterationCount++;
        }
        return new List<Vector3>();
    }

    private Vector3 Support(Vector3[] verticesA, Vector3[] verticesB, Vector3 direction)
    {
        Vector3 pointA = GetFarthestPoint(verticesA, direction);
        Vector3 pointB = GetFarthestPoint(verticesB, -direction);
        return pointA - pointB;
    }

    private Vector3 GetFarthestPoint(Vector3[] vertices, Vector3 direction)
    {
        float maxDot = float.MinValue;
        Vector3 farthestPoint = Vector3.zero;
        foreach (Vector3 vertex in vertices)
        {
            float dot = Vector3.Dot(vertex, direction);

            if (dot > maxDot)
            {
                maxDot = dot;
                farthestPoint = vertex;
            }
        }

        return farthestPoint;
    }

    private bool ContainsOrigin(Vector3[] simplex, ref int simplexSize, ref Vector3 direction)
    {

        if (simplexSize == 2)
        {
            return ContainsOriginLine(ref simplex, ref direction);
        }
        else if (simplexSize == 3)
        {
            return ContainsOriginTriangle(ref simplex, ref direction);
        }

        return false;
    }

    private bool ContainsOriginLine(ref Vector3[] simplex, ref Vector3 direction)
    {
        Vector3 pointA = simplex[1];
        Vector3 pointB = simplex[0];

        Vector3 AB = pointB - pointA;
        Vector3 AO = -pointA;


        if (Vector3.Dot(AB, AO) > 0)
        {
            direction = Vector3.Cross(Vector3.Cross(AB, AO), AB);
            simplex[0] = pointB;
            simplex[1] = Vector3.zero;
            return false;
        }

        direction = AO;
        simplex[1] = Vector3.zero;
        return true;
    }

    private bool ContainsOriginTriangle(ref Vector3[] simplex, ref Vector3 direction)
    {
        Vector3 pointA = simplex[2];
        Vector3 pointB = simplex[1];
        Vector3 pointC = simplex[0];
        Vector3 AB = pointB - pointA;
        Vector3 AC = pointC - pointA;
        Vector3 AO = -pointA;
        Vector3 ABC = Vector3.Cross(AB, AC);

        if (Vector3.Dot(Vector3.Cross(ABC, AB), AO) > 0)
        {
            direction = Vector3.Cross(Vector3.Cross(AB, AO), AB);
            simplex[0] = pointB;
            simplex[1] = pointA;
            simplex[2] = Vector3.zero;
            return false;
        }


        if (Vector3.Dot(Vector3.Cross(AC, ABC), AO) > 0)
        {

            direction = Vector3.Cross(Vector3.Cross(AC, AO), AC);
            simplex[0] = pointC;
            simplex[1] = pointA;
            simplex[2] = Vector3.zero;
            return false;
        }


        direction = ABC;
        return true;
    }

    private List<Vector3> ExtractCollisionPoints(Vector3[] simplex, int simplexSize)
    {
        List<Vector3> collisionPoints = new List<Vector3>();
        collisionPoints.Add(simplex[simplexSize - 1]);
        collisionPoints.Add(simplex[simplexSize - 2]);
        return collisionPoints;
    }

    private void ResolveCollision(GameObject object1, GameObject object2, List<Vector3> collisionPoints)
    {
        Rigidbody rb1 = object1.GetComponent<Rigidbody>();
        Rigidbody rb2 = object2.GetComponent<Rigidbody>();
        if (rb1 == null || rb2 == null)
        {
            return;
        }

        float totalMass = rb1.mass + rb2.mass;

        foreach (Vector3 point in collisionPoints)
        {
            Vector3 collisionNormal = (object1.transform.position - object2.transform.position).normalized;
            Vector3 relativeVelocity = rb1.GetPointVelocity(point) - rb2.GetPointVelocity(point);
            float relativeVelocityAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);
            if (relativeVelocityAlongNormal > 0f)
            {
                return;
            }
            float impulseMagnitude = -(1.0f + restitution) * relativeVelocityAlongNormal / totalMass;
            rb1.AddForceAtPosition(impulseMagnitude * collisionNormal, point, ForceMode.Impulse);
            rb2.AddForceAtPosition(-impulseMagnitude * collisionNormal, point, ForceMode.Impulse);
        }
        rb1.useGravity = true;
        rb2.useGravity = true;
    }

    private void ApplyCollisionResponse(Rigidbody rb, Vector3 collisionNormal)
    {
        Vector3 relativeVelocity = rb.velocity;
        float relativeVelocityAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);
        Vector3 impulse = -(1.0f + restitution) * relativeVelocityAlongNormal * collisionNormal;
        rb.AddForce(impulse, ForceMode.Impulse);
        rb.AddRelativeTorque(-rb.angularVelocity, ForceMode.VelocityChange);
    }
}
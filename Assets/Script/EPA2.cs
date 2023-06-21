using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EPA2 : MonoBehaviour
{
    public float detectionRadius = 2f;
    public float restitution = 0.8f;
    public LayerMask objectLayer;
    public BoundingSphere boundingSphere;
    private void Start()
    {
        boundingSphere = new BoundingSphere(transform.position, detectionRadius);
    }
    private void Update()
    {
        CheckCollisions();
    }
    private void CheckCollisions()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject)
            {
                GameObject object1 = gameObject;
                GameObject object2 = obj;
                BoundingSphere objBoundingSphere = new BoundingSphere(obj.transform.position, detectionRadius);
                if (SphereCollisionCheck(boundingSphere, objBoundingSphere) && !(object2.CompareTag("planwe")) && !(object1.CompareTag("planwe")))
                {
                    if (AABBCollisionCheck(object1, object2))
                    {
                        List<Vector3> collisionPoints = GJKCollisionCheck(object1, object2);
                        if (collisionPoints.Count > 0)
                        {
                            List<Vector3> contactPoints = Epa(collisionPoints, object1, object2);
                            if (contactPoints.Count > 0)
                            {
                                ResolveCollision(object1, object2, contactPoints);
                            }
                            
                        }
                    }
                }
            }
        }
    }
    private bool SphereCollisionCheck(BoundingSphere sphere1, BoundingSphere sphere2)
    {
        float distance = Vector3.Distance(sphere1.center, sphere2.center);
        return distance <= sphere1.radius + sphere2.radius;
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
        Vector3[] simplex = new Vector3[4];
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
        else if (simplexSize == 4)
        {
            return ContainsOriginTetrahedron(ref simplex, ref simplexSize, ref direction);
        }
        return false;
    }
    private bool ContainsOriginLine(ref Vector3[] simplex, ref Vector3 direction)
    {
        Vector3 pointA = simplex[1];
        Vector3 pointB = simplex[0];
        Vector3 AB = pointB - pointA;
        Vector3 AO = -pointA;
        direction = Vector3.Cross(AB, AO);
        if (Vector3.Dot(direction, AO) > 0)
        {
            direction = AO;
        }
        else
        {
            simplex[0] = pointB;
            direction = AO;
        }
        return false;
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
    private bool ContainsOriginTetrahedron(ref Vector3[] simplex, ref int simplexSize, ref Vector3 direction)
    {
        Vector3 pointA = simplex[3];
        Vector3 pointB = simplex[2];
        Vector3 pointC = simplex[1];
        Vector3 pointD = simplex[0];
        Vector3 AB = pointB - pointA;
        Vector3 AC = pointC - pointA;
        Vector3 AD = pointD - pointA;
        Vector3 AO = -pointA;
        Vector3 ABC = Vector3.Cross(AB, AC);
        Vector3 ACD = Vector3.Cross(AC, AD);
        Vector3 ADB = Vector3.Cross(AD, AB);
        if (Vector3.Dot(ABC, AO) > 0)
        {
            direction = ABC;
            simplex[0] = pointA;
            simplex[1] = pointB;
            simplex[2] = pointC;
            simplexSize = 3;
            return false;
        }
        if (Vector3.Dot(ACD, AO) > 0)
        {
            direction = ACD;
            simplex[0] = pointA;
            simplex[1] = pointC;
            simplex[2] = pointD;
            simplexSize = 3;
            return false;
        }
        if (Vector3.Dot(ADB, AO) > 0)
        {
            direction = ADB;
            simplex[0] = pointA;
            simplex[1] = pointD;
            simplex[2] = pointB;
            simplexSize = 3;
            return false;
        }
        return true;
    }
    private List<Vector3> ExtractCollisionPoints(Vector3[] simplex, int simplexSize)
    {
        List<Vector3> collisionPoints = new List<Vector3>();
        for (int i = 0; i < simplexSize; i++)
        {
            collisionPoints.Add(simplex[i]);
        }
        return collisionPoints;
    }
    private List<Vector3> Epa(List<Vector3> simplex, GameObject objectA, GameObject objectB)
    {
        Mesh meshA = objectA.GetComponent<MeshFilter>().sharedMesh;
        Mesh meshB = objectB.GetComponent<MeshFilter>().sharedMesh;
        List<Tetrahedron> tetrahedrons = new List<Tetrahedron>();
        if (simplex.Count == 4)
        {
            tetrahedrons.Add(new Tetrahedron(simplex[0], simplex[1], simplex[2], simplex[3]));
            tetrahedrons.Add(new Tetrahedron(simplex[0], simplex[2], simplex[3], simplex[1]));
        }
        else if (simplex.Count == 3)
        {
            tetrahedrons.Add(new Tetrahedron(simplex[0], simplex[1], simplex[2], Vector3.zero));
        }
        else
        {
            throw new System.ArgumentException("Simplex must have at least 3 points and at most 4 points");
        }
        int maxIterations = 100;
        int iterationCount = 0;
        while (iterationCount < maxIterations)
        {
            Tetrahedron closestTetrahedron = FindClosestTetrahedron(tetrahedrons);
            Vector3 supportPoint = Support(meshA.vertices, meshB.vertices, closestTetrahedron.normal);
            float distanceToTetrahedron = Vector3.Dot(closestTetrahedron.normal, supportPoint) - closestTetrahedron.distance;
            if (distanceToTetrahedron < 0f)
            {
                return new List<Vector3>();
            }
            tetrahedrons.Remove(closestTetrahedron);
            CreateTetrahedrons(supportPoint, closestTetrahedron, tetrahedrons);
            bool originInside = tetrahedrons.Any(t => IsOriginInsideTetrahedron(t));
            if (originInside)
            {
                List<Vector3> collisionPoints = ExtractCollisionPointstetrahedrons(tetrahedrons);
                return collisionPoints;
            }
            iterationCount++;
        }
        return new List<Vector3>();
    }
    private static void CreateTetrahedrons(Vector3 supportPoint, Tetrahedron closestTetrahedron, List<Tetrahedron> tetrahedrons)
    {
        tetrahedrons.Clear();
        if (closestTetrahedron.IsTriangle)
        {
            tetrahedrons.Add(new Tetrahedron(supportPoint, closestTetrahedron.A, closestTetrahedron.B, closestTetrahedron.C));
        }
        else
        {
            tetrahedrons.Add(new Tetrahedron(supportPoint, closestTetrahedron.A, closestTetrahedron.B, closestTetrahedron.C));
            tetrahedrons.Add(new Tetrahedron(supportPoint, closestTetrahedron.B, closestTetrahedron.C, closestTetrahedron.D));
            tetrahedrons.Add(new Tetrahedron(supportPoint, closestTetrahedron.C, closestTetrahedron.D, closestTetrahedron.A));
            tetrahedrons.Add(new Tetrahedron(supportPoint, closestTetrahedron.D, closestTetrahedron.A, closestTetrahedron.B));
        }
    }
    private bool IsOriginInsideTetrahedron(Tetrahedron tetrahedron)
    {
        Vector3 p1 = tetrahedron.A;
        Vector3 p2 = tetrahedron.B;
        Vector3 p3 = tetrahedron.C;
        Vector3 p4 = tetrahedron.D;
        bool sameSide123 = SameSide(p1, p2, p3, p4);
        bool sameSide124 = SameSide(p1, p2, p4, p3);
        bool sameSide134 = SameSide(p1, p3, p4, p2);
        bool sameSide234 = SameSide(p2, p3, p4, p1);
        return sameSide123 && sameSide124 && sameSide134 && sameSide234;
    }
    private bool SameSide(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 p1p2 = p2 - p1;
        Vector3 p1p3 = p3 - p1;
        Vector3 p1p4 = p4 - p1;
        Vector3 crossP1 = Vector3.Cross(p1p2, p1p3);
        Vector3 crossP2 = Vector3.Cross(p1p2, p1p4);
        float dotP1 = Vector3.Dot(crossP1, crossP2);
        return dotP1 >= 0f;
    }
    private List<Vector3> ExtractCollisionPointstetrahedrons(List<Tetrahedron> tetrahedrons)
    {
        List<Vector3> collisionPoints = new List<Vector3>();
        foreach (Tetrahedron tetrahedron in tetrahedrons)
        {
            collisionPoints.Add(tetrahedron.A);
            collisionPoints.Add(tetrahedron.B);
            collisionPoints.Add(tetrahedron.C);
            if (!tetrahedron.IsTriangle)
            {
                collisionPoints.Add(tetrahedron.D);
            }
        }
        return collisionPoints;
    }
    public Tetrahedron FindClosestTetrahedron(List<Tetrahedron> tetrahedrons)
    {
        float closestDistance = float.MaxValue;
        Tetrahedron closestTetrahedron = null;
        Vector3 origin = Vector3.zero;
        foreach (var tetrahedron in tetrahedrons)
        {
            Vector3 tetrahedronCenter = (tetrahedron.A + tetrahedron.B + tetrahedron.C + tetrahedron.D) / 4f;
            float distance = Vector3.Distance(origin, tetrahedronCenter);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTetrahedron = tetrahedron;
            }
        }
        return closestTetrahedron;
    }
    private void ResolveCollision(GameObject object1, GameObject object2, List<Vector3> collisionPoints)
    {

        Rigidbody rb1 = object1.GetComponent<Rigidbody>();
        Rigidbody rb2 = object2.GetComponent<Rigidbody>();
        if (rb1 == null || rb2 == null)
        {
            return;
        }
        foreach (Vector3 point in collisionPoints)
        {
            Vector3 collisionNormal = (object1.transform.position - object2.transform.position).normalized;
            Vector3 relativeVelocity = rb1.GetPointVelocity(point) - rb2.GetPointVelocity(point);
            float relativeVelocityAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);
            if (relativeVelocityAlongNormal > 0f)
            {
                return;
            }
            float impulseScalar = (-(1f + 1f) * relativeVelocityAlongNormal) / (1f / rb1.mass + 1f / rb2.mass);
            impulseScalar += impulseScalar * Time.deltaTime;
            rb1.velocity += impulseScalar / rb1.mass * collisionNormal;
            rb2.velocity -= impulseScalar / rb2.mass * collisionNormal;
        }
    }
}

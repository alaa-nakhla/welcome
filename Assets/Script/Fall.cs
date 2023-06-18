using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Fall : MonoBehaviour
{
    public float detectionRadius = 2f;
    public float restitution = 0.8f;
    public LayerMask objectLayer;
    private List<GameObject> nearestObjects;
    private Rigidbody rb;
    // Start is called before the first frame update
    private void Start()
    {
        nearestObjects = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        FindNearestObjects();
        CheckCollisions();
        if (CheckCollisions())
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
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
                    // Debug.Log(obj.name);
                    nearestObjects.Add(obj);
                }
            }
        }
    }
    private bool CheckCollisions()
    {
        for (int i = 0; i < nearestObjects.Count; i++)
        {
            // Perform collision detection logic here using AABB algorithm
            Renderer renderer1 = gameObject.GetComponent<Renderer>();
            Renderer renderer2 = nearestObjects[i].GetComponent<Renderer>();
            if (renderer1 != null && renderer2 != null)
            {
                Vector3 min1 = renderer1.bounds.min;
                Vector3 max1 = renderer1.bounds.max;
                Vector3 min2 = renderer2.bounds.min;
                Vector3 max2 = renderer2.bounds.max;
                // Perform AABB collision check
                if (max1.x >= min2.x && min1.x <= max2.x &&
                    max1.y >= min2.y && min1.y <= max2.y &&
                    max1.z >= min2.z && min1.z <= max2.z)
                {
                    if (nearestObjects[i].name != "floor")
                    {
                        //Debug.Log(nearestObjects[i].name);
                    }

                    if (nearestObjects[i].name == "floor" || gameObject.name == "floor")
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    private void ResolveCollision(GameObject object1, GameObject object2, float restitution)
    {
        Rigidbody rb1 = object1.GetComponent<Rigidbody>();
        Rigidbody rb2 = object2.GetComponent<Rigidbody>();

        if (rb1 != null && rb2 != null)
        {
            Vector3 relativeVelocity = rb2.velocity - rb1.velocity;
            Vector3 collisionNormal = (rb2.position - rb1.position).normalized;

            // Calculate relative velocity along the collision normal
            float relativeVelocityAlongNormal = Vector3.Dot(relativeVelocity, collisionNormal);

            // Check if the objects are moving away from each other
            if (relativeVelocityAlongNormal > 0)
            {
                return; // Objects are already separating, no collision response needed
            }

            // Calculate the effective mass of the objects
            float effectiveMass1 = 1.0f / rb1.mass;
            float effectiveMass2 = 1.0f / rb2.mass;

            // Calculate the impulse scalar
            float impulseScalar = -(1.0f + restitution) * relativeVelocityAlongNormal /
                (effectiveMass1 + effectiveMass2);

            // Calculate the impulse vectors
            Vector3 impulse1 = impulseScalar * collisionNormal * effectiveMass1;
            Vector3 impulse2 = impulseScalar * collisionNormal * effectiveMass2;

            // Apply impulses to the objects
            rb1.AddForce(impulse1, ForceMode.Impulse);
            rb2.AddForce(impulse2, ForceMode.Impulse);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CollisionDetection : MonoBehaviour
{
    public float collisionDistanceThreshold = 1f;
    private void Start()
    {
    }
    private void Update()
    {
        CheckCollisions();
    }
    private bool AABBCollisionCheck(GameObject object1, GameObject object2)
    {
        Renderer renderer1 = object1.GetComponent<Renderer>();
        Renderer renderer2 = object2.GetComponent<Renderer>();
        if (renderer1 != null && renderer2 != null)
        {
            // Get the center of each object
            Vector3 center1 = object1.transform.position;
            Vector3 center2 = object2.transform.position;

            // Calculate the extents of each object (half the width, height, and depth)
            Vector3 extents1 = renderer1.bounds.extents;
            Vector3 extents2 = renderer2.bounds.extents;

            // Calculate the minimum and maximum bounds of the AABB for each object
            Vector3 min1 = center1 - extents1;
            Vector3 max1 = center1 + extents1;
            Vector3 min2 = center2 - extents2;
            Vector3 max2 = center2 + extents2;

            // Check for overlap
            if (max1.x >= min2.x && min1.x <= max2.x &&
                max1.y >= min2.y && min1.y <= max2.y &&
                max1.z >= min2.z && min1.z <= max2.z)
            {
                return true;
            }
        }
        return false;
    }
 
    private bool SphereCollisionCheck(GameObject object1, GameObject object2)
    {
        Renderer renderer1 = object1.GetComponent<Renderer>();
        Renderer renderer2 = object2.GetComponent<Renderer>();
        if (renderer1 != null && renderer2 != null)
        {
            Vector3 center1 = renderer1.bounds.center;
            Vector3 center2 = renderer2.bounds.center;
            float radius1 = renderer1.bounds.extents.magnitude;
            float radius2 = renderer2.bounds.extents.magnitude;
            float distance = Vector3.Distance(center1, center2);
            if (distance <= radius1 + radius2)
            {
                return true;
            }
        }
        return false;
    }

  /*  public bool AABBCollisionCheck(GameObject object1, GameObject object2)
    {
        Renderer renderer1 = object1.GetComponent<Renderer>();
        Renderer renderer2 = object2.GetComponent<Renderer>();
        if (renderer1 != null && renderer2 != null)
        {
            Vector3 min1 = renderer1.bounds.min;
            Vector3 max1 = renderer1.bounds.max;
            Vector3 min2 = renderer2.bounds.min;
            Vector3 max2 = renderer2.bounds.max;
            Vector3[] axes = new Vector3[3];
            axes[0] = object2.transform.right;
            axes[1] = object2.transform.up;
            axes[2] = object2.transform.forward;
            for (int i = 0; i < axes.Length; i++)
            {
                Vector3 axis = axes[i];
                float min1_proj = Vector3.Dot(axis, min1);
                float max1_proj = Vector3.Dot(axis, max1);
                float min2_proj = Vector3.Dot(axis, min2);
                float max2_proj = Vector3.Dot(axis, max2);
                if (!(max2_proj >= min1_proj && max1_proj >= min2_proj))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }*/
    public void CheckCollisions()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        GameObject object1 = gameObject;
       
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject)
            {
                GameObject object2 = obj;
                float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
                if (distance < collisionDistanceThreshold)
                {
                    if (AABBCollisionCheck(object1, object2))
                    {
                        if (!object1.CompareTag("model") && !object2.CompareTag("plan"))
                        {
                            object1.GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezePositionY;

                        }
                    }
                    else
                    {
                        if (!object1.CompareTag("model") && !object2.CompareTag("plan"))
                        {
                            object1.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
                        }
                    }
                }

            }
        }
    }
}

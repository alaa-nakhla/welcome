using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycForAngry : MonoBehaviour
{
    public float raycastDistance = 1f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            if (gameObject != null && GetComponent<Rigidbody>() != null)
            {
                float y = rb.velocity.y;
                rb.velocity = new Vector3(rb.velocity.x, -rb.velocity.y, rb.velocity.z);
            }
        }
    }
}
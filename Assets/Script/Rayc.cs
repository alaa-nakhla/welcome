using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rayc : MonoBehaviour
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
           //rb.velocity = new Vector3(rb.velocity.x, 0f,rb.velocity.z);
             rb.constraints |= RigidbodyConstraints.FreezePositionY;            
        }
    }
}

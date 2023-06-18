using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rayc : MonoBehaviour
{
    public float speed = 1f;
    public float raycastDistance = 1f;
    public LayerMask groundLayer;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // إطلاق الأشعة لأسفل من المكعب
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            rb.velocity = new Vector3(rb.velocity.x, -rb.velocity.y, rb.velocity.z);
            //rb.AddForce(movement * speed);
            //rb.isKinematic = false;
        }
    }
}
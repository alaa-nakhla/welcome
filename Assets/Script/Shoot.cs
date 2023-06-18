using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            // Draw a red line from the camera to the hit point
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * hit.distance, Color.red);
        }

    }
}

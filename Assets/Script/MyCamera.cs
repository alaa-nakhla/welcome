using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left"))
        {
            transform.Translate(-0.1f, 0, 0);
        }
        if (Input.GetKey("right"))
        {
            transform.Translate(0.1f, 0, 0);
        }
        if (Input.GetKey("up"))
        {
            transform.Translate(0, 0.1f, 0);
        }
        if (Input.GetKey("down"))
        {
            transform.Translate(0, -0.1f, 0);
        }
        if (Input.GetKey("w"))
        {
            transform.Translate(0, 0, 0.1f);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(0, 0, -0.1f);
        }
        if (Input.GetKey("a"))
        {
            transform.Rotate(0, -0.1f, 0);
        }
        if (Input.GetKey("d"))
        {
            transform.Rotate(0, 0.1f, 0);
        }
    }
}

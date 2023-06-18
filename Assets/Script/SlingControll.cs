using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingControll : MonoBehaviour
{
    public float sensitivityY = 15f;
    public float minimumY = -60f;
    public float maximumY = 60f;
    public float rotationY = 0f;
    public float speedX = 10f;
    
    // Update is called once per frame

    void Update()
    {

        if (Input.GetKey("q"))
        {
            transform.Translate(-speedX * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("e"))
        {
            transform.Translate(speedX * Time.deltaTime, 0, 0);
        }
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}

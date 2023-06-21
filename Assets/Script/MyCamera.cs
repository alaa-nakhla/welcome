using UnityEngine;

public class MyCamera : MonoBehaviour
{
    private float y;

    void Start()
    {
        y = transform.position.y;
    }
    void Update()
    {
        if (Input.GetKey("left"))
        {
            transform.Translate(-0.2f, 0, 0);
        }
        if (Input.GetKey("right"))
        {
            transform.Translate(0.2f, 0, 0);
        }
        if (Input.GetKey("up"))
        {
            y = Mathf.Clamp(y + 0.05f, 1, 50);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        if (Input.GetKey("down"))
        {
            y = Mathf.Clamp(y - 0.05f, 1, 50);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
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

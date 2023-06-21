using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiat : MonoBehaviour
{
    public GameObject box;
    float counter;
    Vector3 position = new Vector3(-60f, 2f, -30f);
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if (Input.GetKey(KeyCode.F) && counter >= 1f)
        {
            counter = 0f;
            Instantiate(box, position, Quaternion.identity);
            position.x += 30f;

        }

    }
}

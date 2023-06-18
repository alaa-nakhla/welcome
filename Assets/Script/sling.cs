using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sling : MonoBehaviour
{
    public Transform LeftPoint;
    public Transform RightPoint;
    public Transform CenterPoint;
    LineRenderer slingShotString;
    // Start is called before the first frame update
    void Start()
    {
        slingShotString = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        slingShotString.SetPositions(new Vector3[3] { LeftPoint.position, CenterPoint.position, RightPoint.position });
    }
}

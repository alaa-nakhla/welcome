using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounseBall : MonoBehaviour
{
    // Start is called before the first frame update
    //private AudioSource bounce;
    public GameObject Bouncing;
    void Start()
    {
        // bounce=gameObject.GetComponent<AudioSource>();    
    }
    // Update is called once per frame
    void Update()
    {
    }
    void OnTriggerEnter(Collider other)
    {
        Bouncing.gameObject.GetComponent<AudioSource>().Play();
    }
}

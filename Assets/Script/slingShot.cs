using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slingShot : MonoBehaviour
{
    public Transform Projectle;
    public Transform DrawFrom;
    public Transform DrawTo;
    public sling slingShotString;
    public int NrDrawIcrements = 2;
    private static Transform currentProjectile;
    private bool draw;
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    private AudioSource audioSource;
    // Start is called before the first frame update

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void PlaySound1()
    {
        audioSource.clip = audioClip1;
        audioSource.Play();
    }
    void PlaySound2()
    {
        audioSource.clip = audioClip2;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //slingShotString.CenterPoint
        if (Input.GetMouseButtonDown(0))
        {
            DrawSlingShot(1);
            PlaySound1();
        }
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseAndShot(40);
            PlaySound2();
        }
    }
    public void ReleaseAndShot(float shotForce)
    {
        draw = false;
        currentProjectile.transform.parent = null;
        Rigidbody projecitleRigidBody = currentProjectile.GetComponent<Rigidbody>();
        projecitleRigidBody.AddForce(transform.forward * shotForce, ForceMode.Impulse);
        slingShotString.CenterPoint = DrawFrom;
        Object.DontDestroyOnLoad(currentProjectile.GetComponent<Rigidbody>());
        Destroy(currentProjectile.GetComponent<Rigidbody>(), 5f);
        // Find all instances of the prefab in the scene
    }
    public void DrawSlingShot(float speed)
    {
        draw = true;
        currentProjectile = Instantiate(Projectle, DrawFrom.position, Quaternion.identity, transform);
        slingShotString.CenterPoint = currentProjectile.transform;
        float waitTimeBetweenDraws = speed / NrDrawIcrements;
        StartCoroutine(DrawSlingShotWithIncrements(waitTimeBetweenDraws));
        GameObject[] instances = GameObject.FindGameObjectsWithTag("Finish");
        // Destroy each instance
        foreach (GameObject instance in instances)
        {
            Destroy(instance, 5f);
        }
    }
    private IEnumerator DrawSlingShotWithIncrements(float waitTimeBetweenDraws)
    {
        for (int i = 0; i < NrDrawIcrements; i++)
        {
            if (draw)
            {
                currentProjectile.transform.position = Vector3.Lerp(DrawFrom.position, DrawTo.position, (float)i / NrDrawIcrements);
                yield return new WaitForSeconds(waitTimeBetweenDraws);
            }
            else
            {
                i = NrDrawIcrements;
            }
        }
    }
}

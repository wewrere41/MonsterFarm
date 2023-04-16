//
//
//NOTES:
//
//This script is used for DEMONSTRATION porpuses of the Projectiles. I recommend everyone to create their own code for their own projects.
//THIS IS JUST A BASIC EXAMPLE PUT TOGETHER TO DEMONSTRATE VFX ASSETS.
//
//


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoveScript : MonoBehaviour
{
    public bool rotate = false;
    public float rotateAmount = 45;
    public bool bounce = false;
    public float bounceForce = 10;
    public float speed;
    [Tooltip("From 0% to 100%")] public float accuracy;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    private Vector3 startPos;
    private float speedRandomness;
    private Vector3 offset;
    private bool collided;
    private Rigidbody rb;
    private RotateToMouseScript rotateToMouse;
    private GameObject target;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();

        //used to create a radius for the accuracy and have a very unique randomness
        if (accuracy != 100)
        {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 2; i++)
            {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 2);
                if (i == 0)
                {
                    if (index == 0)
                        offset = new Vector3(0, -val, 0);
                    else
                        offset = new Vector3(0, val, 0);
                }
                else
                {
                    if (index == 0)
                        offset = new Vector3(0, offset.y, -val);
                    else
                        offset = new Vector3(0, offset.y, val);
                }
            }
        }

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void FixedUpdate()
    {
        if (target != null)
            rotateToMouse.RotateToMouse(gameObject, target.transform.position);
        if (rotate)
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        if (speed != 0 && rb != null)
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.CompareTag("Ground") && !collided)
        {
            collided = true;


            speed = 0;
            GetComponent<Rigidbody>().isKinematic = true;

            var pos = co.ClosestPoint(transform.position);


            var hitVFX = Instantiate(hitPrefab, pos, Quaternion.identity);
            var ps = hitVFX.GetComponentInChildren<ParticleSystem>();
            Destroy(hitVFX, ps.main.duration);

            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject trg, RotateToMouseScript rotateTo)
    {
        target = trg;
        rotateToMouse = rotateTo;
    }
}
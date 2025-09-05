using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    public GameObject explosionPrefab;
    private Rigidbody rb;
    void Start()
    {
         rb = GetComponent<Rigidbody>();

        // Move forward using physics
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    // Void Update()
    // {
    //     transform.Translate(Vector3.forward * speed * Time.deltaTime);
    // }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
    {
          
        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        return;
    }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

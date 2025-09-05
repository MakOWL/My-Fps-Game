using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
  [Header("Explosion Settings")]
    private SphereCollider sphereCollider;
    public float maxRadius = 20f;     // How big the explosion should grow
    public float expansionSpeed = 10f; // How fast it grows
    public float lifeTime = 1f;   
    public AudioSource audioSource;
    public AudioClip explosionSound;
  // Start is called before the first frame update

  void Start()
  {
    sphereCollider = GetComponent<SphereCollider>();
    if (sphereCollider == null)
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
    }
    sphereCollider.isTrigger = true;
    sphereCollider.radius = 4f;

    // Destroy explosion after lifetime
    Destroy(gameObject, lifeTime);
  }
  void Update()
  {
    // Smoothly expand radius until max
        if (sphereCollider.radius < maxRadius)
        {
            sphereCollider.radius += expansionSpeed * Time.deltaTime;
        } 
  }

   void OnTriggerEnter(Collider other)
    {
        Debug.Log("Explosion hit: " + other.name);
    }

    public void OnExplosionEnd()
  {
    Destroy(gameObject); // removes explosion from the scene
  }
    public void OnExplosionStart()
    {
      audioSource.PlayOneShot(explosionSound);
    }
}

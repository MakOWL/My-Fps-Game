using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    //public GameObject explosionPrefab; // drag your explosion prefab here
    public float explosionForce = 500f;
    public float explosionRadius = 5f;
    public float damage = 50f;

    private bool exploded = false;

    // Call this when your bullet hits the barrel
     public GameObject explosionEffect;
    public AudioSource audioSource;
    public AudioClip fireSound;

    public void Start()
    {
       // explosionEffect.SetActive(false);
    }
    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
        {
            Vector3 offset = new Vector3(0f, 2f, 0f);
            Instantiate(explosionEffect, transform.position + offset, Quaternion.identity);
            audioSource.PlayOneShot(fireSound);
        }

        // Apply physics to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            // ExplosiveBarrel barrel = hit.collider.GetComponent<ExplosiveBarrel>();
            // if (barrel != null)
            // {
            //     barrel.Explode(); // simple explosion
            // }

        }
        Destroy(gameObject);
    }

    // Detect weapon hit
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("explosion")) // Or your weapon's ray hit logic
        {
            Explode();
        }
    }
}

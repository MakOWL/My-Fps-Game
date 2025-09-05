using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class smgMP5 : MonoBehaviour
{
    [Header("Weapon Sprites")]
    public Image weaponImage;
    public Sprite idleSprite;
    public Sprite[] fireSprites;
    public float frameTime = 0.05f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;

    [Header("Muzzle Flash Light")]
    public Light muzzleFlashLight;  // Assign a light in the inspector
    public float lightDuration = 0.05f; // How long the light stays on

    private bool isAnimating = false;

    public LineRenderer lineRenderer;

    [Header("Bullet Impact")]
    public GameObject bulletImpactPrefab; // Assign the WarFX bullet hole prefab here

    public SimpleSpriteAnimator spriteAnimator;

    [Header("Fire Rate")]
    public float fireRate = 0.1f; // Time between bullets
    private float nextFireTime = 0f;


    void Start()
    {
        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false; // Make sure it's off initially
    }

void Update()
{
    if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
    {
        nextFireTime = Time.time + fireRate;
        StartCoroutine(FireAnimation());
    }
}

    IEnumerator FireAnimation()
    {
        isAnimating = true;

        // Play fire sound
        if (fireSound != null)
            audioSource.PlayOneShot(fireSound);

        // Enable muzzle flash light temporarily
        if (muzzleFlashLight != null)
            StartCoroutine(MuzzleFlashLightEffect());

        ShootHitscan();

        // Continue with your normal sprite firing animation
        int num = 0;
        foreach (Sprite s in fireSprites)
        {
            // num += 1;
            // if (num == 7){spriteAnimator.PlayAnimation();}
            weaponImage.sprite = s;
            yield return new WaitForSeconds(frameTime);
        }

        weaponImage.sprite = idleSprite;
        isAnimating = false;
    }

    IEnumerator MuzzleFlashLightEffect()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleFlashLight.enabled = false;
    }

    void ShootHitscan()
{
    float range = 60f;
    RaycastHit hit;

    Vector3 start = Camera.main.transform.position;
    Vector3 end = start + Camera.main.transform.forward * range;

    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
    {
        Debug.Log("Hit: " + hit.collider.name);
        end = hit.point;

        // 🔹 Spawn bullet mark
        if (bulletImpactPrefab != null)
        {
            // Offset a tiny bit so it doesn’t clip into the surface
            Vector3 spawnPos = hit.point + hit.normal * 0.01f;
            Quaternion spawnRot = Quaternion.LookRotation(hit.normal);

            // Parent it to the hit object so it moves with it
            Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
        }

        // You could also spawn sparks/blood depending on tag
        // if (hit.collider.CompareTag("Enemy")) { ... spawn blood effect ... }
    }
    else
    {
        Debug.Log("Missed!");
    }

    // Optional tracer
    // StartCoroutine(ShowBulletLine(start, end));
}


    IEnumerator ShowBulletLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.05f);

        lineRenderer.enabled = false;
    }
}

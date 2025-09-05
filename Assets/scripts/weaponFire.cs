using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class weaponFire : MonoBehaviour
{
    [Header("Weapon Sprites")]
    public Image weaponImage;
    public Sprite idleSprite;
    public Sprite[] fireSprites;
    public int[] alwaysLitUpSprites;
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

    [Header("Weapon Type")]
    public int weapon;

    enum weapon_type
    {
        PISTOL,
        SHOTGUN,
        SMG,
        ROCKET_LAUNCHER
    }

    [Header("Fire Rate (for SMG)")]
    public float fireRate = 0.1f; // Time between SMG shots
    private float nextFireTime = 0f;

    [Header("Rocket Launcher ")]
    public GameObject rocket;
    public Transform firePoint;

    void Start()
    {
        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false; // Make sure it's off initially
    }
    [Header("Sprite Effects")]
    public float brightness = 1f;
    public shadowZoneDectector szd;

    void shadow_effect(float b)
    {
        Color originalColor = weaponImage.color;
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        brightness = b;
        v = brightness; // set brightness in HSV
        Color darkenedColor = Color.HSVToRGB(h, s, v);
        weaponImage.color = darkenedColor;
    }

    void Update()
    {
        if (!isAnimating)  // Only apply zone effect if not in fire animation
        {
            if (szd.isShadow)
                shadow_effect(0.5f);
            else
                shadow_effect(1f);
        }
        if (weapon == (int)weapon_type.SMG)
        {
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                StartCoroutine(FireAnimation());
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !isAnimating)
            {
                StartCoroutine(FireAnimation());
            }
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

        Fire();
        for (int i = 0; i < fireSprites.Length; i++)
        {
            weaponImage.sprite = fireSprites[i];

            // Check if this sprite index is in alwaysLitUpSprites

            if (szd.isShadow)
            {
                if (System.Array.IndexOf(alwaysLitUpSprites, i) >= 0)
                {
                    // Force full brightness
                    shadow_effect(1.0f);
                }
                else
                {
                    // Let shadow system darken it normally
                    shadow_effect(0.5f); // <- whatever your shadow value is
                }
            }

            yield return new WaitForSeconds(frameTime);

        }

        weaponImage.sprite = idleSprite;
        if (szd.isShadow)
            shadow_effect(0.5f);
        else
            shadow_effect(1f);


        isAnimating = false;
    }

    void Fire()
    {

        if (weapon == (int)weapon_type.PISTOL)
        {
            ShootHitscan_Single_shot();
        }
        else if (weapon == (int)weapon_type.SHOTGUN)
        {
            ShootHitscan_sg_shot();
        }

        else if (weapon == (int)weapon_type.SMG)
        {
            shadow_effect(1f);
            ShootHitscan_SMG();
            
        }
        else if (weapon == (int)weapon_type.ROCKET_LAUNCHER)
        {
            ShootRocket();
        }


    }
    void ShootRocket()
    {
        // this will shoot
        Camera cam = Camera.main;
        GameObject projectile = Instantiate(rocket, firePoint.position, Quaternion.identity);
        projectile.transform.rotation = Quaternion.LookRotation(cam.transform.forward);

        //Instantiate(rocket, firePoint.position, firePoint.rotation);

        Collider playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
        Collider rocketCollider = rocket.GetComponent<Collider>();

        if (playerCollider != null && rocketCollider != null)
        {
            Physics.IgnoreCollision(rocketCollider, playerCollider);
        }
    }

    IEnumerator MuzzleFlashLightEffect()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleFlashLight.enabled = false;
    }

    void ShootHitscan_sg_shot()
    {
        int pelletCount = 8;       // How many pellets per shot
        float spreadAngle = 5f;    // Degrees of spread
        float range = 60f;

        Vector3 start = Camera.main.transform.position;

        for (int i = 0; i < pelletCount; i++)
        {
            // Random spread direction
            Vector3 direction = Camera.main.transform.forward;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            RaycastHit[] hits = Physics.RaycastAll(start, direction, range);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("ShadowZone") || hit.collider.CompareTag("ShadowDetector"))
                {
                    // Skip shadow zone, keep checking further
                    continue;
                }

                Debug.Log("Hit: " + hit.collider.name);

                if (bulletImpactPrefab != null)
                {
                    Vector3 spawnPos = hit.point + hit.normal * 0.01f;
                    Quaternion spawnRot = Quaternion.LookRotation(hit.normal);
                    Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
                }

                ExplosiveBarrel barrel = hit.collider.GetComponent<ExplosiveBarrel>();
                if (barrel != null)
                {
                    barrel.Explode(); // simple explosion
                }
                // Stop after hitting first valid target
                break;
            }


        }
    }
    void ShootHitscan_SMG()
    {
        int pelletCount = 1;       // How many pellets per shot
        float spreadAngle = 1f;    // Degrees of spread
        float range = 60f;

        Vector3 start = Camera.main.transform.position;

        for (int i = 0; i < pelletCount; i++)
        {
            // Random spread direction
            Vector3 direction = Camera.main.transform.forward;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            RaycastHit[] hits = Physics.RaycastAll(start, direction, range);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("ShadowZone") || hit.collider.CompareTag("ShadowDetector"))
                {
                    // Skip shadow zone, keep checking further
                    continue;
                }

                Debug.Log("Hit: " + hit.collider.name);

                if (bulletImpactPrefab != null)
                {
                    Vector3 spawnPos = hit.point + hit.normal * 0.01f;
                    Quaternion spawnRot = Quaternion.LookRotation(hit.normal);
                    Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
                }

                ExplosiveBarrel barrel = hit.collider.GetComponent<ExplosiveBarrel>();
                if (barrel != null)
                {
                    barrel.Explode(); // simple explosion
                }
                break;
            }


        }
    }
    void ShootHitscan_Single_shot()
    {
        int pelletCount = 1;       // How many pellets per shot
        float spreadAngle = 0f;    // Degrees of spread
        float range = 60f;

        Vector3 start = Camera.main.transform.position;

        for (int i = 0; i < pelletCount; i++)
        {
            // Random spread direction
            Vector3 direction = Camera.main.transform.forward;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            RaycastHit[] hits = Physics.RaycastAll(start, direction, range);

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("ShadowZone") || hit.collider.CompareTag("ShadowDetector"))
                {
                    // Skip shadow zone, keep checking further
                    continue;
                }

                Debug.Log("Hit: " + hit.collider.name);

                if (bulletImpactPrefab != null)
                {
                    Vector3 spawnPos = hit.point + hit.normal * 0.01f;
                    Quaternion spawnRot = Quaternion.LookRotation(hit.normal);
                    Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
                }

                ExplosiveBarrel barrel = hit.collider.GetComponent<ExplosiveBarrel>();
                if (barrel != null)
                {
                    barrel.Explode(); // simple explosion
                }
                break;
            }
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
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class shotgun : MonoBehaviour
{
    [Header("Weapon Sprites")]
    public Image weaponImage;
    public SpriteRenderer idleSprite;
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

    //[Range(0f, 1f)] public float brightness = 1f;
    [Header("sprtie brightness")]
    public float brightness = 1f;
    public lightEffects lf;

    void Start()
    {
        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false; // Make sure it's off initially
    }

    void shadow_effect(float b)
    {
        Color originalColor = idleSprite.color;
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);
        brightness = b;
        v = brightness; // set brightness in HSV
        Color darkenedColor = Color.HSVToRGB(h, s, v);
        idleSprite.color = darkenedColor;
    }


    void Update()
    {
        if (lf.isShadow)
        {
            shadow_effect(0.5f);
        }
        else
        {
            shadow_effect(1f);
        }
        Color originalColor = weaponImage.color;
        Color.RGBToHSV(originalColor, out float h, out float s, out float v);

        v = brightness; // set brightness in HSV
        Color darkenedColor = Color.HSVToRGB(h, s, v);
        weaponImage.color = darkenedColor;

        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
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
            weaponImage.sprite = s;
            yield return new WaitForSeconds(frameTime);
        }

        weaponImage.sprite = idleSprite.sprite;
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

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, range))
        {
               if (hit.collider.CompareTag("ShadowZone"))
                {
                    continue;
                }
            Debug.Log("Hit: " + hit.collider.name);

            // 🔹 Spawn bullet mark
            if (bulletImpactPrefab != null)
            {
                Vector3 spawnPos = hit.point + hit.normal * 0.01f;
                Quaternion spawnRot = Quaternion.LookRotation(hit.normal);
                Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
            }
        }
        else
        {
            Debug.Log("Missed pellet!");
        }
    }
}

    //     void ShootHitscan()
    // {
    //     float range = 60f;
    //     RaycastHit hit;

    //     Vector3 start = Camera.main.transform.position;
    //     Vector3 end = start + Camera.main.transform.forward * range;

    //     if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
    //     {
    //         Debug.Log("Hit: " + hit.collider.name);
    //         end = hit.point;

    //         // 🔹 Spawn bullet mark
    //         if (bulletImpactPrefab != null)
    //         {
    //             // Offset a tiny bit so it doesn’t clip into the surface
    //             Vector3 spawnPos = hit.point + hit.normal * 0.01f;
    //             Quaternion spawnRot = Quaternion.LookRotation(hit.normal);

    //             // Parent it to the hit object so it moves with it
    //             Instantiate(bulletImpactPrefab, spawnPos, spawnRot, hit.collider.transform);
    //         }

    //         // You could also spawn sparks/blood depending on tag
    //         // if (hit.collider.CompareTag("Enemy")) { ... spawn blood effect ... }
    //     }
    //     else
    //     {
    //         Debug.Log("Missed!");
    //     }

    //     // Optional tracer
    //     // StartCoroutine(ShowBulletLine(start, end));
    // }


    IEnumerator ShowBulletLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.05f);

        lineRenderer.enabled = false;
    }
}

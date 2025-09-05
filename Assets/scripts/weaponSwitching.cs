using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject[] weapons; // Drag weapon GameObjects here

    private int currentWeaponIndex = 0;

    void Start()
    {
        UpdateWeaponDisplay();
    }

    
    void Update()
    {
        // Scroll wheel up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            UpdateWeaponDisplay();
        }

        // Scroll wheel down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
                currentWeaponIndex = weapons.Length - 1;
            UpdateWeaponDisplay();
        }

        // Number keys 1, 2, 3...
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                currentWeaponIndex = i;
                UpdateWeaponDisplay();
            }
        }

        // Middle mouse button to cycle
        if (Input.GetMouseButtonDown(2))
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            UpdateWeaponDisplay();
        }
    }

    void UpdateWeaponDisplay()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }
    }
}

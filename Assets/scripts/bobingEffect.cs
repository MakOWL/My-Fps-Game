using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bobingEffect : MonoBehaviour
{
    public RectTransform weaponSprite; // Assign the RectTransform of the weapon sprite
    public float bobFrequency = 5.0f;  // How fast the bobbing occurs
    public float bobAmplitude = 10.0f; // The bobbing distance in pixels
    private Vector2 initialPosition;
    private float timer = 0.0f;



    void Start()
    {
        // Store the initial position of the weapon sprite
        initialPosition = weaponSprite.anchoredPosition;
    }

    
    void Update()
    {
        
        if (IsPlayerMoving()) // Replace with your movement detection logic
        {
            // Increment timer based on bobFrequency
            bobFrequency = 5.0f;
            timer += Time.deltaTime * bobFrequency;

            // Calculate the new offsets for bobbing
            float offsetX = Mathf.Sin(timer) * bobAmplitude;
            float offsetY = Mathf.Cos(timer * 2) * (bobAmplitude * 0.5f);

            // Apply the offsets to the weapon sprite's position
            weaponSprite.anchoredPosition = initialPosition + new Vector2(offsetX, offsetY);
        }
        else
        {
            // Reset position and timer when the player is not moving
            timer = 0.0f;
            weaponSprite.anchoredPosition = initialPosition;
        }
    }

    private bool IsPlayerMoving()
    {
        // Replace with your actual player movement logic
        // Example: Input detection for movement
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
}

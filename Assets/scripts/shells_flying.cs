using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleSpriteAnimator : MonoBehaviour
{
    public Image targetImage;       // The UI Image or SpriteRenderer to animate
    public Sprite[] frames;         // Array of sprites to play
    public float frameDelay = 0.1f; // Time in seconds between frames

    private Coroutine animationCoroutine;

    // Call this method to start the animation
    public void PlayAnimation()
    {
        // If already playing, stop it first
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlayAnimationCoroutine());
    }

    private IEnumerator PlayAnimationCoroutine()
    {
        foreach (Sprite frame in frames)
        {
            targetImage.sprite = frame;
            yield return new WaitForSeconds(frameDelay);
        }
    }
}

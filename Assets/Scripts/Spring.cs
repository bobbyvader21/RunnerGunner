using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // Sprite to show when the spring is pressed
    [SerializeField] Sprite _sprung;

    // Reference to the object's SpriteRenderer
    SpriteRenderer _spriteRenderer;

    // Stores the original/default sprite
    Sprite _defaultSprite;
    
    AudioSource _audioSource;

    void Awake()
    {
        // Cache the SpriteRenderer component (runs once at start)
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original sprite so we can switch back later
        _defaultSprite = _spriteRenderer.sprite;

        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // When something collides with this object
        // Check if that object is the Player
        if (collision.collider.CompareTag("Player"))
        {
            // Change sprite to "sprung" (pressed spring)
            _spriteRenderer.sprite = _sprung;

            _audioSource.Play();
        }
            
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // When the Player leaves the collision
        if (collision.collider.CompareTag("Player"))

            // Reset sprite back to default (unpressed spring)
            _spriteRenderer.sprite = _defaultSprite;
    }
}
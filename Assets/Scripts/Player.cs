using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Tracks when the jump should stop (for variable jump height)
    float _jumpEndTime;

    // Movement speed (left/right)
    [SerializeField] float _horizontalVelocity = 3;

    // Jump strength (upward velocity)
    [SerializeField] float _jumpVelocity = 5;

    // How long the jump lasts when holding the button
    [SerializeField] float _jumpDuration = 0.5f;

    // Sprite used when jumping (not currently applied here, but available)
    [SerializeField] Sprite _jumpSprite;

    // Layer mask to detect ONLY ground (important for raycasts)
    [SerializeField] LayerMask _layerMask;

    // Offset for left/right foot ground detection
    [SerializeField] float _footOffset = 0.5f;

    // Public variable to track if player is on the ground
    public bool IsGrounded;

    // Cached components (for performance)
    SpriteRenderer _spriteRenderer;
    AudioSource _audioSource;
    Animator _animator;

    // Stores horizontal input value
    float _horizontal;

    // Tracks how many jumps remain (for double jump)
    int _jumpsRemaining;

    private void Awake()
    {
        // Cache components ONCE (better than calling every frame)
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    void OnDrawGizmos()
    {
        // Draw debug lines in Scene view to visualize raycasts
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Gizmos.color = Color.red;

        // Center ray (middle foot)
        Vector2 origin = new Vector2(
            transform.position.x,
            transform.position.y - spriteRenderer.bounds.extents.y
        );
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Left foot ray
        origin = new Vector2(
            transform.position.x - _footOffset,
            transform.position.y - spriteRenderer.bounds.extents.y
        );
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        // Right foot ray
        origin = new Vector2(
            transform.position.x + _footOffset,
            transform.position.y - spriteRenderer.bounds.extents.y
        );
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
    }

    // Called once per frame
    void Update()
    {
        // Check if player is grounded using raycasts
        UpdateGrounding();

        // Get horizontal input (-1 left, 1 right)
        _horizontal = Input.GetAxis("Horizontal");

        // Debug input value
        Debug.Log(_horizontal);

        // Get Rigidbody2D for movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Keep current vertical velocity (so gravity still applies)
        var vertical = rb.velocity.y;

        // If jump is pressed AND player still has jumps remaining
        if (Input.GetButtonDown("Jump") && _jumpsRemaining > 0)
        {
            // Set jump timer for variable height
            _jumpEndTime = Time.time + _jumpDuration;

            // Reduce remaining jumps (for double jump system)
            _jumpsRemaining--;

            // Change pitch depending on jump number
            if (_jumpsRemaining > 0)
                _audioSource.pitch = 1;       // normal jump
            else
                _audioSource.pitch = 1.2f;   // second jump

            // Play jump sound
            _audioSource.Play();
        }

        // If jump button is held AND within allowed jump time
        if (Input.GetButton("Jump") && _jumpEndTime > Time.time)
        {
            // Apply upward velocity (keeps player rising)
            vertical = _jumpVelocity;
        }

        // Apply horizontal movement speed
        _horizontal *= _horizontalVelocity;

        // Apply final velocity to Rigidbody
        rb.velocity = new Vector2(_horizontal, vertical);

        // Update animations and sprite direction
        UpdateSprite();
    }

    void UpdateGrounding()
    {
        // Assume player is NOT grounded initially
        IsGrounded = false;

        // Get bottom-center of player (feet position)
        Vector2 origin = new Vector2(
            transform.position.x,
            transform.position.y - _spriteRenderer.bounds.extents.y
        );

        // Cast ray downward (center)
        var hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);

        if (hit.collider)
            IsGrounded = true;

        // Check left foot
        origin = new Vector2(
            transform.position.x - _footOffset,
            transform.position.y - _spriteRenderer.bounds.extents.y
        );
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);

        if (hit.collider)
            IsGrounded = true;

        // Check right foot
        origin = new Vector2(
            transform.position.x + _footOffset,
            transform.position.y - _spriteRenderer.bounds.extents.y
        );
        hit = Physics2D.Raycast(origin, Vector2.down, 0.1f, _layerMask);

        if (hit.collider)
            IsGrounded = true;

        // If grounded AND not moving upward, reset jumps (double jump reset)
        if (IsGrounded && GetComponent<Rigidbody2D>().velocity.y <= 0)
            _jumpsRemaining = 2;
    }

    void UpdateSprite()
    {
        // Tell animator whether player is grounded
        _animator.SetBool("IsGrounded", IsGrounded);

        // Send horizontal speed (absolute value for animation blending)
        _animator.SetFloat("HorizontalSpeed", Math.Abs(_horizontal));

        // Flip sprite depending on movement direction
        if (_horizontal > 0)
            _spriteRenderer.flipX = false;
        else if (_horizontal < 0)
            _spriteRenderer.flipX = true;
    }
}
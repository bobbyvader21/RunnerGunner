using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float _jumpEndTime;
    [SerializeField] float _horizontalVelocity = 3;
    [SerializeField] float _jumpVelocity = 5;
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] Sprite _jumpSprite;
    public bool IsGrounded;
    SpriteRenderer _spriteRenderer;
    Sprite _defaultSprite;
    float _horizontal;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _spriteRenderer.sprite;
    }

    void OnDrawGizmos()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.extents.y);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the SpriteRenderer to access the player's size
        // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Create a point at the bottom of the player (feet)
        // transform.position is center, so subtract half the height
        Vector2 origin = new Vector2(
            transform.position.x,
            transform.position.y - _spriteRenderer.bounds.extents.y
        );

        // Cast a short ray downward to check if ground is directly below
        var hit = Physics2D.Raycast(origin, Vector2.down, 0.1f);

        // If the ray hits something, the player is grounded
        if (hit.collider)
            IsGrounded = true;
        else
            IsGrounded = false;

        // Get horizontal input (-1 = left, 0 = idle, 1 = right)
        _horizontal = Input.GetAxis("Horizontal");

        // Print input value to console (for debugging)
        Debug.Log(_horizontal);

        // Get Rigidbody2D component (controls physics movement)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Store current vertical velocity (so gravity still applies)
        var vertical = rb.velocity.y;

        // If jump button is pressed AND player is on the ground
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            // Set how long the jump can continue (for variable height)
            _jumpEndTime = Time.time + _jumpDuration;
        }

        // If jump button is being held AND within allowed jump time
        if (Input.GetButton("Jump") && _jumpEndTime > Time.time)
        {
            // Apply upward velocity (jumping)
            vertical = _jumpVelocity;
        }

        // Apply final movement:
        // horizontal input for x, calculated vertical for y
        _horizontal *= _horizontalVelocity;
        rb.velocity = new Vector2(_horizontal, vertical);

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (IsGrounded)
            _spriteRenderer.sprite = _defaultSprite;
        else
            // Make jump sprite for character
            _spriteRenderer.sprite = _jumpSprite;

        // Flips jump sprite depending on key press
        if (_horizontal > 0)
            _spriteRenderer.flipX = false;
        else if (_horizontal < 0)
            _spriteRenderer.flipX = true;
    }
}

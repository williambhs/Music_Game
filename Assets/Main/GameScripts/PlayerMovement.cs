using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] int playerNum = 1;

    // healthbar
    public float Hitpoints;
    public float MaxHitpoints = 10;
    public HealthbarBehavior Healthbar;

    // audio
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource damagedSoundEffect;
    [SerializeField] private AudioSource shootingSoundEffect;


    // dashing stuff
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.5f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;

    // projectile stuff
    public ProjectileBehavior ProjectilePrefab;
    [SerializeField] Vector3 launchOffset;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState {idle, running, jumping, falling}

    public void TakeHit(float damage)
    {
        damagedSoundEffect.Play();
        Hitpoints -= damage;
        Healthbar.SetHealth(Hitpoints, MaxHitpoints);
        
        // change this to game over once done
        if (Hitpoints <= 0)
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // healthbar stuff
        Hitpoints = MaxHitpoints;
        Healthbar.SetHealth(Hitpoints, MaxHitpoints);
    }

    // default update method
    private void Update()
    {
        //movement
        dirX = Input.GetAxisRaw(String.Format("P{0}Horizontal", playerNum));
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown(String.Format("P{0}Jump", playerNum)) && IsGrounded())
        {
            jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // shooting
        if (Input.GetButtonDown(String.Format("Fire{0}", playerNum)))
        {
            shootingSoundEffect.Play();
            ProjectileBehavior p = Instantiate(ProjectilePrefab.gameObject, transform.position + new Vector3(launchOffset.x * (sprite.flipX ? 1 : -1), launchOffset.y, launchOffset.z), transform.rotation).GetComponent<ProjectileBehavior>();
            if (!sprite.flipX)
            {
                p.speed *= -1;
            }
        }

        // dashing
        if (isDashing)
        {
            return;
        }
        if (Input.GetButtonDown(String.Format("P{0}Dash", playerNum)) && canDash)
        {
            StartCoroutine(Dash());
        }
        UpdateAnimationState();

       
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running; 
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        //Debug.Log(dashingPower);
        int temp;
        if (sprite.flipX) 
        {
            temp = -1;
        }
        else 
        {
            temp = 1;
        }
        rb.velocity = new Vector2(temp * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    
} 

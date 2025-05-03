using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Rigidbody2D playerRigidBody;
    private float baseHealth;
    public float healthMultiplier;
    public Animator animator;
    private BoxCollider2D playerBoxCollider;

    [Header("Movement")]
    private float horizontalMovement;
    public float moveSpeedMultiplier;
    private float jumpsRemaining;
    public float maxJumps;
    private bool facingRight;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);

    [Header("Firing")]
    private Vector2 lookDirection;
    public float cardsLeft;

    private void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        horizontalMovement = 0f;
        moveSpeedMultiplier = 5f;
        baseHealth = 100f;
        healthMultiplier = 1f;
        maxJumps = 2;
        jumpsRemaining = 2;
        facingRight = true;
        animator = GetComponent<Animator>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
        playerBoxCollider.size = new Vector2(0.75f, 1.2f);
        lookDirection = new Vector2(1f, 0f);
        cardsLeft = 0;
    }

    public void Move(InputAction.CallbackContext context) {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (jumpsRemaining > 0) {
            if (context.performed) {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 12f);
                jumpsRemaining--;
                animator.SetTrigger("isJumping");
            } else if (context.canceled) {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 6f);
                jumpsRemaining--;
                animator.SetTrigger("isJumping");
            }
        }
    }

    public void Crouch(InputAction.CallbackContext context) {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer) && context.performed) {
            
            
            playerBoxCollider.size = new Vector2(0.75f, 0.6f);
            playerBoxCollider.offset = new Vector2(-0.05f, -0.4f);
            moveSpeedMultiplier = 1f;
            animator.SetBool("isCrouching", true);
        }
        if (context.canceled) {
            playerBoxCollider.size = new Vector2(0.75f, 1.2f);
            playerBoxCollider.offset = new Vector2(-0.05f, -0.11f);

            moveSpeedMultiplier = 5f;
            animator.SetBool("isCrouching", false);
        }
        
    }

    public void Look(InputAction.CallbackContext context) {
        lookDirection = context.ReadValue<Vector2>();
    }

    public void Fire(InputAction.CallbackContext context) {
        // set animation, set blocker
        if(cardsLeft == 0) {
            
        }
    }


    private void Update() {
        playerRigidBody.velocity = new Vector2(horizontalMovement * moveSpeedMultiplier, playerRigidBody.velocity.y);
        GroundChecker();
        animator.SetFloat("xVelocity", Mathf.Abs(playerRigidBody.velocity.x));
        animator.SetFloat("yVelocity", playerRigidBody.velocity.y);

    }

    private void FixedUpdate() {   
        if (horizontalMovement < 0 && facingRight) {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            facingRight = false;
        }
        if (horizontalMovement > 0 && !facingRight) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            facingRight = true;
        }
    }

    private void GroundChecker() {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)) {
            jumpsRemaining = maxJumps;
        }
    }

}

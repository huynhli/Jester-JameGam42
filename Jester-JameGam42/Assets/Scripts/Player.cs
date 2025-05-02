using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Rigidbody2D playerRigidBody;
    private float baseHealth;
    public float healthMultiplier;
    public Animator animator;

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
    }

    public void Move(InputAction.CallbackContext context) {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (jumpsRemaining > 0) {
            if (context.performed) {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 15f);
                jumpsRemaining--;
                animator.SetTrigger("isJumping");
            } else if (context.canceled) {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 8f);
                jumpsRemaining--;
                animator.SetTrigger("isJumping");
            }
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

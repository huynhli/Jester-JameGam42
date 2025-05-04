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
    public int cardsLeft;
    private float attackType;
    private int cardsInHandIndex;
    public GameManager gameManager;
    public Camera mainCamera;

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
        lookDirection = Vector2.right;
        cardsInHandIndex = 0;
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
        Vector2 mouseScreenPos = context.ReadValue<Vector2>();
        Vector2 worldMousePos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        lookDirection = worldMousePos - playerRigidBody.position;
    }

    public void Fire(InputAction.CallbackContext context) {
        // set animation, set blocker
        if(cardsLeft == 0) {
            animator.SetFloat("AttackType", 0);
        } else if (context.performed) {
            // set animation
            setAttackType(lookDirection);
            animator.SetFloat("AttackType", attackType);
            gameManager.UseDefenceCard(cardsInHandIndex, lookDirection, playerRigidBody.position);
            // zero or end case
            if (cardsLeft > 0 && cardsLeft == cardsInHandIndex) {
                cardsInHandIndex--;
            }
        }
    }

    public void SelectCardToRight(InputAction.CallbackContext context) {
        if(context.performed) {
            if (cardsInHandIndex + 1 == cardsLeft) {
                cardsInHandIndex = 0;
            } else {
                cardsInHandIndex++;
            }
        }
    }

    public void SelectCardToLeft(InputAction.CallbackContext context) {
        if(context.performed) {
             if (cardsInHandIndex == 0) {
            cardsInHandIndex = cardsLeft - 1;
            } else {
                cardsInHandIndex--;
            }
        }
        
       

    }

    public void AlternateFire(InputAction.CallbackContext context) {
        // TODO if right click, shoot the card in a thin line --> if collides with card circle, 
    }

    private void setAttackType(Vector2 directionLooking) {
        // look right
        if (directionLooking.x >= 0) {

        }
        attackType = 1;
        // look left
        // look up
        // look down
    }

    public void ResetPlayer() {
        transform.position = new Vector3(0, 0, 0);
        playerRigidBody.velocity = Vector2.zero;        // Reset velocity to 0
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

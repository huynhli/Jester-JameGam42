using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Rigidbody2D playerRigidBody;
    private float baseHealth;
    public float healthMultiplier;
    public Animator animator;
    private BoxCollider2D playerBoxCollider;
    public Text healthNum;
    public float totalHealth;

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

    private void OnEnable() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        horizontalMovement = 0f;
        moveSpeedMultiplier = 7f;
        baseHealth = 100f;
        healthMultiplier = 1f;
        totalHealth = baseHealth * healthMultiplier;
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
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 20f);
                jumpsRemaining--;
                animator.SetTrigger("isJumping");
            } else if (context.canceled) {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 10f);
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
        if (context.performed) {
            if(cardsLeft == 0) {
                animator.SetTrigger("attackNull");
            } else {
                // set animation
                setAttackType(lookDirection);
                gameManager.UseDefenceCard(cardsInHandIndex, lookDirection, playerRigidBody.position);
                // edge case
                if (cardsLeft > 0 && cardsLeft == cardsInHandIndex) {
                    cardsInHandIndex--;
                }
                
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
        
        if (directionLooking.x >= 1) {
            if (!facingRight) {
                StartCoroutine(FlipTemporarily(1f, 0.2f));
            }
            animator.SetTrigger("attackRight");
        } else if (directionLooking.x <= -1) {
            if (facingRight) {
                StartCoroutine(FlipTemporarily(-1f, 0.3f));
            }
            animator.SetTrigger("attackLeft");
        } else if (directionLooking.y >= 1) {
            animator.SetTrigger("attackUp");
        } else if (directionLooking.y <= -1) {
            animator.SetTrigger("attackDown");
        } else {
            animator.SetTrigger("attackRight");
        }
    }

    private IEnumerator FlipTemporarily(float xDir, float duration) {
        Vector3 original = transform.localScale;
        transform.localScale = new Vector3(xDir * Mathf.Abs(original.x), original.y, original.z);
        yield return new WaitForSeconds(duration);
        transform.localScale = original;
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

    public void Knockback(Vector2 directionToKnock, int magnitude) {
        playerRigidBody.AddForce(directionToKnock*magnitude*500);
    }

    public void Reset() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        horizontalMovement = 0f;
        moveSpeedMultiplier = 7f;
        baseHealth = 100f;
        healthMultiplier = 1f;
        totalHealth = baseHealth * healthMultiplier;
        maxJumps = 2;
        jumpsRemaining = 2;
        facingRight = true;
        animator = GetComponent<Animator>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
        playerBoxCollider.size = new Vector2(0.75f, 1.2f);
        lookDirection = Vector2.right;
        cardsInHandIndex = 0;
    }

    public void UpdateHealth(float dmgTaken) {
        totalHealth = totalHealth - dmgTaken;
        healthNum.text = totalHealth.ToString();
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

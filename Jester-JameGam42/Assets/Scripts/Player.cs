using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player")]
    private Rigidbody2D playerRigidBody;
    private float baseHealth;
    public float healthMultiplier;

    [Header("Movement")]
    private float horizontalMovement;
    public float moveSpeedMultiplier;
    private float jumpsRemaining;
    public float maxJumps;

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
    }

    public void Move(InputAction.CallbackContext context) {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }


    private void Update() {
        playerRigidBody.velocity = new Vector2(horizontalMovement * moveSpeedMultiplier, playerRigidBody.velocity.y);
        GroundChecker();
    }

    private void FixedUpdate() {

    }

    private void GroundChecker() {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)) {
            jumpsRemaining = maxJumps;
        }
    }

}

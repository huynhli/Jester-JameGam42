using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int spriteNum;
    private int cardHealth;
    private SpriteRenderer spriteRenderer;
    private Vector2 directionToShoot;
    private Rigidbody2D attackCardRigidBody;
    private float speed = 5f;
    
    public Sprite[] attackSprites;

    private void Awake() {
        // Safe to cache references in Awake for instantiated prefabs
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackCardRigidBody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int random, Vector2 directionToPlayer){
        spriteRenderer.sprite = attackSprites[random];
        spriteNum = random;
        cardHealth = random + 2;
        directionToShoot = -directionToPlayer.normalized;
        StartCoroutine(MoveAfterDelay());
    }

    private void changeHealth(int newHealth) {
        cardHealth = newHealth;
        spriteNum = newHealth-2;
        spriteRenderer.sprite = attackSprites[spriteNum];
        spriteRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }

    private IEnumerator MoveAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        attackCardRigidBody.velocity = directionToShoot * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            StartCoroutine(LodgeAndDestroy());
        } else if (other.CompareTag("DefenceCard")) {
            DefenceCard defendingCard = other.GetComponent<DefenceCard>();
            int defendingCardHealth = defendingCard.GetHealth();
            int diff = cardHealth - defendingCardHealth;
            if (cardHealth > defendingCardHealth) {
                changeHealth(diff);
                Destroy(other.gameObject);
            } else if (cardHealth < defendingCardHealth) {
                defendingCard.changeHealth(-diff);
                Destroy(gameObject);
            } else {
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
        } else if (other.CompareTag("Player")) {
            Player playerObject = other.GetComponent<Player>();
            playerObject.UpdateHealth(cardHealth);
            playerObject.Knockback(directionToShoot.normalized, cardHealth);
            Destroy(gameObject);
        }
    }

    private IEnumerator LodgeAndDestroy() {
        attackCardRigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

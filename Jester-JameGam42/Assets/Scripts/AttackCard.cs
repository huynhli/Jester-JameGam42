using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int spriteNum;
    [SerializeField] private AudioClip spawnSoundClip;
    [SerializeField] private AudioClip playerDmgSoundClip;
    [SerializeField] private AudioClip blockedSoundClip;

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
        directionToShoot = -directionToPlayer;
        SoundManager.instance.PlaySoundFXClip(spawnSoundClip, transform, 0.5f);
        StartCoroutine(MoveAfterDelay());
    }

    private void changeHealth(int newHealth) {
        if (newHealth == 1) {
            cardHealth = 2;
            spriteRenderer.sprite = attackSprites[0];
            return;
        }
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
            SoundManager.instance.PlaySoundFXClip(blockedSoundClip, transform, 3f);
            DefenceCard defendingCard = other.GetComponent<DefenceCard>();
            int defendingCardHealth = defendingCard.GetHealth();
            if (cardHealth > defendingCardHealth) {
                changeHealth(cardHealth - defendingCardHealth);
                Destroy(other.gameObject);
            } else if (cardHealth < defendingCardHealth) {
                defendingCard.changeHealth(defendingCardHealth - cardHealth);
                Destroy(gameObject);
            } else {
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
        } else if (other.CompareTag("Player")) {
            SoundManager.instance.PlaySoundFXClip(playerDmgSoundClip, transform, 3f);
            Player playerObject = other.GetComponent<Player>();
            playerObject.UpdateHealth(cardHealth);
            playerObject.Knockback(directionToShoot.normalized, cardHealth);
            Destroy(gameObject);
        }
    }

    private IEnumerator LodgeAndDestroy() {
        attackCardRigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}

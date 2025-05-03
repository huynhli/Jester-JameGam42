using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int cardHealth;
    private SpriteRenderer spriteRenderer;
    
    public Sprite[] attackSprites;

    private void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int random){
        spriteRenderer.sprite = attackSprites[random];
        cardHealth = random;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {

        } else if (other.CompareTag("DefenceCard")) {

        } else if (other.CompareTag("Player")) {

        }
    }
}

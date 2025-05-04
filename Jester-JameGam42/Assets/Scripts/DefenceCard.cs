using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int cardHealth;
    public int spriteNum;
    public Sprite[] cardSprites;
    private SpriteRenderer spriteRenderer;

    private void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int GetHealth() {
        return cardHealth;
    }

    public void Initialize(int random) {
        spriteRenderer.sprite = cardSprites[random];
        spriteNum = random;
        cardHealth = random + 2;
    }
    
    public void changeHealth(int newHealth) {
        cardHealth = newHealth;
        spriteNum = newHealth-2;
        spriteRenderer.sprite = cardSprites[spriteNum];
        spriteRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }
}

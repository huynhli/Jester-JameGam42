using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceCard : MonoBehaviour
{
    // Start is called before the first frame update
    private int cardHealth;
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
        cardHealth = random;
    }
}
